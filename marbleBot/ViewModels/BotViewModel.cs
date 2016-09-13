using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Media;

namespace marbleBot.ViewModels
{
	public class BotViewModel : Utils.ViewModel
	{
		#region Static
		public static List<string> PropertiesInUpdateList()
		{
			if (propertyUpdates != null)
			{
				return null;
			}

			propertyUpdates = new List<string>();
			foreach (var prop in typeof(BotViewModel).GetProperties())
			{
				if (prop.GetCustomAttributes(typeof(ManualUpdateAttribute), false).Length > 0)
				{
					propertyUpdates.Add(prop.Name);
				}
			}
			return propertyUpdates;
		}
		protected static List<String> propertyUpdates;
		#endregion

		#region Instance

		public BotViewModel()
		{
			PropertiesInUpdateList();


			bot = new Models.Bot();
						

			twitchMessages = new System.Collections.ObjectModel.ObservableCollection<string>();
			(bot.bot.EH.operators.Find((a) => a.Item1 is Models.BotCommands.WPFPrettyPrint).Item1 as Models.BotCommands.WPFPrettyPrint).twitchChat = twitchMessages;
			
			BindingOperations.EnableCollectionSynchronization(twitchMessages, twitchMessageLock);
			BindingOperations.EnableCollectionSynchronization(ranks, rankLock);

			tick.Interval = TimeSpan.FromSeconds(1);

			tick.Tick += (a, b) =>
			{
				foreach (var name in propertyUpdates)
				{
					UpdateProperty(name);
				}
			};

			tick.Start();

			Models.BotCommands.GetRank.NewRank callback = cmd =>
			{
				if (cmd.username.ToLower() == bot.cred.nickname.ToLower())
				{
					myRank = cmd;
				}

				if (cmd.position >= ranks.Count)
				{
					ranks.Add(cmd);
				}
				else {
					ranks[cmd.position - 1] = cmd;
					
				}

				UpdateProperty(nameof(nextRank));

			};

			Models.BotCommands.TopTen.OnNewRank += callback;
			Models.BotCommands.GetRank.OnNewRank += callback;
		}


		public void SetRank(string pos, string total, string points)
		{
			currentRank = String.Format("{0} / {1} - {2}", pos, total, points);
		}

		Models.Bot bot;

		DispatcherTimer tick = new DispatcherTimer();

		#endregion
		#region WPF Bindings

		public object rankLock = new object();

		public System.Collections.ObjectModel.ObservableCollection<Models.Rank> ranks
		{
			get
			{
				return GetProperty() as System.Collections.ObjectModel.ObservableCollection<Models.Rank> ??
					SetProperty(new System.Collections.ObjectModel.ObservableCollection<Models.Rank>());
			}
		}

		public System.ComponentModel.ICollectionView ranksSorted
		{
			get
			{
				ListCollectionView prop = GetProperty() as ListCollectionView;
				if(prop == null)
				{
					prop = SetProperty(CollectionViewSource.GetDefaultView(ranks) as ListCollectionView);
					prop.CustomSort = new Models.Rank.RankSortPosition();
				}
				
				return prop;
				
			}
		}

		public Models.Rank myRank
		{
			get
			{
				return GetProperty() as Models.Rank ?? SetProperty(new Models.Rank { username = "", points = 0, position = 0 });
			}
			set
			{
				SetProperty(value);
			}
		}

		[ManualUpdate]
		public Utils.ViewCommand startStopBot
		{
			get
			{
				if (!IsBotRunning)
				{
					return startBot;
				}
				else
				{
					return stopBot;
				}
			}
		}

		[ManualUpdate]
		public string botStatusString
		{
			get
			{
				if (!IsBotRunning)
				{
					return "Start Bot";
				}
				else
				{
					return "Stop Bot";
				}
			}
		}

		[ManualUpdate]
		public bool IsBotRunning
		{
			get
			{
				return !(bot.botTask == null || bot.botTask.Status != TaskStatus.Running);
			}
		}

		//[ManualUpdate]
		public Utils.ViewCommand startBot
		{
			get
			{
				return GetProperty() as Utils.ViewCommand ??
					SetProperty<Utils.ViewCommand>(
						new Utils.ViewCommand(
							(obj) =>
							{
								bot.StartBot(
									new TwitchBot.Classes.Credentials(credNick, credPass)
								);
							},
							(obj) => credNick != "" && credPass != "" && !IsBotRunning
						)
					);
			}
		}

		[ManualUpdate]
		public Utils.ViewCommand stopBot
		{
			get
			{
				return GetProperty() as Utils.ViewCommand ??
					SetProperty<Utils.ViewCommand>(
						new Utils.ViewCommand((obj) =>
						{
							bot.StopBot();
						},
						obj => IsBotRunning
						)
					);
			}
		}

		[ManualUpdate]
		public Utils.ViewCommand joinChannel
		{
			get
			{
				return GetProperty() as Utils.ViewCommand ??
					SetProperty<Utils.ViewCommand>(
							new Utils.ViewCommand((obj) =>
							{
								bot.bot.Join("marbleracing");
							},
							(obj) => !bot.bot.channels.Contains("marbleracing")
						)
					);
			}
		}

		[ManualUpdate]
		public Utils.ViewCommand cmdCheat
		{
			get
			{
				return GetProperty() as Utils.ViewCommand ??
					SetProperty<Utils.ViewCommand>(
							new Utils.ViewCommand((obj) =>
							{
								bot.bot.PM("marbleracing", "!cheat");
							},
							(obj) => bot.bot.channels.Contains("marbleracing")
						)
					);
			}
		}

		[ManualUpdate]
		public Utils.ViewCommand cmdRigged
		{
			get
			{
				return GetProperty() as Utils.ViewCommand ??
					SetProperty<Utils.ViewCommand>(
							new Utils.ViewCommand((obj) =>
							{
								bot.bot.PM("marbleracing", "!rigged");
							},
							(obj) => bot.bot.channels.Contains("marbleracing")
						)
					);
			}
		}

		[ManualUpdate]
		public Utils.ViewCommand cmdRank
		{
			get
			{
				return GetProperty() as Utils.ViewCommand ??
					SetProperty<Utils.ViewCommand>(
							new Utils.ViewCommand((obj) =>
							{
								bot.bot.PM("marbleracing", "!rank");
							},
							(obj) => bot.bot.channels.Contains("marbleracing")
						)
					);
			}
		}

		public string credNick
		{
			get
			{
				return GetProperty() as string;
			}
			set
			{
				SetProperty(value); UpdateProperty(nameof(startStopBot)); UpdateProperty(nameof(startBot));
			}
		}

		public string credPass
		{
			get
			{
				return GetProperty() as string;
			}
			set
			{
				SetProperty(value); UpdateProperty(nameof(startStopBot)); UpdateProperty(nameof(startBot));
			}
		}

		public object twitchMessageLock = new object();

		public System.Collections.ObjectModel.ObservableCollection<string> twitchMessages
		{
			get
			{
				return GetProperty() as System.Collections.ObjectModel.ObservableCollection<string>;
			}
			set
			{
				SetProperty(value);
			}
		}

		public System.Windows.Input.ICommand clearMessages
		{
			get
			{
				return GetProperty() as System.Windows.Input.ICommand ??
					SetProperty(new Utils.ViewCommand(obj => twitchMessages.Clear()));
			}
		}

		[ManualUpdate]
		public string botStatus
		{
			get
			{
				return bot.botTask?.Status.ToString();

			}
		}

		[ManualUpdate]
		public string botError
		{
			get
			{
				return bot.botTask?.Exception.ToString();

			}
		}

		[ManualUpdate]
		public IEnumerable<string> marbleColor
		{
			get
			{
				return Models.BotCommands.EnterRace.marbleList;
			}
		}

		[ManualUpdate]
		public int totalRaces
		{
			get
			{
				return Models.BotCommands.EnterRace.totalRaces;
			}
		}

		public string currentRank
		{
			get
			{
				return GetProperty() as String;
			}
			set
			{
				SetProperty(value);
			}
		}

		public DateTime? lastRank
		{
			get
			{
				return GetProperty() as DateTime?;
			}
			set
			{
				SetProperty(DateTime.Now);
			}
		}

		public int nextRank
		{
			get
			{
				return Models.BotCommands.RankCaller.shoutLimit - Models.BotCommands.RankCaller.shoutCount;
			}
			set
			{

				Models.BotCommands.RankCaller.shoutLimit = Models.BotCommands.RankCaller.shoutCount + (int)value;
				UpdateProperty();
			}
		}

		public double timeLowRange
		{
			get
			{
				return Models.BotCommands.EnterRace.timeLow;
			}
			set
			{
				Models.BotCommands.EnterRace.timeLow = value;
				UpdateProperty();
			}
		}

		public double timeHighRange
		{
			get
			{
				return Models.BotCommands.EnterRace.timeHigh;
			}
			set
			{
				Models.BotCommands.EnterRace.timeHigh = value;
				UpdateProperty();
			}
		}

		[ManualUpdate]
		public double timeSelected
		{
			get
			{
				return TimeSpan.FromMilliseconds(Models.BotCommands.EnterRace.timeSelection).TotalSeconds;
			}

		}

		public string selectedColor
		{
			get
			{
				return Models.BotCommands.EnterRace.cSelect;
			}
			set
			{

				Models.BotCommands.EnterRace.cSelect = value;
				UpdateProperty(nameof(ballColor));
			}
		}

		public Brush ballColor
		{
			get
			{
				var brush = GetProperty() as LinearGradientBrush;
				if (brush == null)
				{
					brush = SetProperty(new LinearGradientBrush());
					brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(selectedColor), 0.0));
					brush.GradientStops.Add(new GradientStop(Color.FromArgb(1, 0, 0, 0), 1.0));
				}
				else
				{
					brush.GradientStops[0].Color = (Color)ColorConverter.ConvertFromString(selectedColor);
				}

				return brush;
			}
		}

		#endregion


	}

	[System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	sealed class ManualUpdateAttribute : Attribute
	{
		// This is a named argument
		public float length { get; set; }
	}
}
