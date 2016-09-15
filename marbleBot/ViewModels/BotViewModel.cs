using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

namespace marbleBot.ViewModels
{
	public class BotViewModel : Utils.ViewModel
	{
		#region Static
		static BotViewModel()
		{
			PropertiesInUpdateList();
		}

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

		Models.Bot bot;

		DispatcherTimer tick = new DispatcherTimer();

		public BotViewModel()
		{
			bot = new Models.Bot();

			dynamic configNickPass = new { nick = "", pass = "" };
			try
			{
				configNickPass = ConfigLoader.LoadConfig("userInfo.json");
			}
			catch (System.IO.FileNotFoundException)
			{
				ConfigLoader.SaveConfig("userInfo.json", configNickPass);
			}
			credNick = configNickPass.nick;
			credPass = configNickPass.pass;

			// Bind twitchchat collection to operator
			twitchMessages = new System.Collections.ObjectModel.ObservableCollection<string>();
			(bot.bot.EH.operators.Find((a) => a.Item1 is Models.BotCommands.WPFPrettyPrint).Item1 as Models.BotCommands.WPFPrettyPrint).twitchChat = twitchMessages;
			
			// Multithread collections
			BindingOperations.EnableCollectionSynchronization(twitchMessages, twitchMessageLock);
			//BindingOperations.EnableCollectionSynchronization(ranks, rankLock);

			// Bind property updates to timer
			tick.Interval = TimeSpan.FromSeconds(1);
			tick.Tick += propertyTick;
			tick.Start();

			// TODO: Improve rank
			// Dictionary of username to point value? 

			// Bind update rank commands
			Models.BotCommands.TopTen.OnNewRank += UpdateRankCallback;
			Models.BotCommands.GetRank.OnNewRank += UpdateRankCallback;
		}

		private void propertyTick(object sender, EventArgs e)
		{
			foreach (var name in propertyUpdates)
			{
				UpdateProperty(name);
			}
		}

		private void UpdateRankCallback(Models.Rank rank)
		{
			if (rank.username.ToLower() == bot.cred.nickname.ToLower())
			{
				myRank = rank;
			}

			((IDictionary<string,Models.Rank>)rankLookup)[rank.username.ToLower()] = rank;
			//UpdateProperty(nameof(ranksSorted));
			//if (rank.position >= ranks.Count)
			//{
			//	ranks.Add(rank);
			//}
			//else
			//{
			//	ranks[rank.position - 1] = rank;

			//}

			UpdateProperty(nameof(nextRank));
		}
		#endregion
		#region WPF Bindings

		public object rankLock = new object();
		
		public DictList<string, Models.Rank> rankLookup
		{
			get
			{
				return GetProperty() as DictList<string, Models.Rank> ?? SetProperty(new DictList<string, Models.Rank>());
			}
		}

		public Dictionary<string, Models.Rank> rankDict
		{
			get
			{
				// CStyle cast should have less overhead
				return (Dictionary < string, Models.Rank > )GetProperty()?? SetProperty(new Dictionary<string, Models.Rank>());
			}
		}

		public System.ComponentModel.ICollectionView ranksSorted
		{
			get
			{
				ListCollectionView prop = GetProperty() as ListCollectionView;
				if(prop == null)
				{
					var p = CollectionViewSource.GetDefaultView((IList<Models.Rank>)rankLookup) as ListCollectionView;
					prop = SetProperty(p as ListCollectionView, false);
					prop.CustomSort = new Models.Rank.RankSortPosition();
					prop.IsLiveSorting = true;
					
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

								if (!(string.IsNullOrWhiteSpace(credNick) && string.IsNullOrWhiteSpace(credPass)))
								{
									ConfigLoader.SaveConfig("userInfo.json", new { nick = credNick, pass = credPass });
								}
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

		public Utils.ViewCommand arbCmd
		{
			get
			{
				return GetProperty() as Utils.ViewCommand ??
					SetProperty<Utils.ViewCommand>(
							new Utils.ViewCommand(param =>
							{
								bot.bot.PM("marbleracing", param as string);
							},
							param => bot.bot.channels.Contains("marbleracing")
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
				return Models.BotCommands.EnterRace.TotalRaces;
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
				return Models.BotCommands.EnterRace.TimeLow;
			}
			set
			{
				Models.BotCommands.EnterRace.TimeLow = value;
				UpdateProperty();
			}
		}

		public double timeHighRange
		{
			get
			{
				return Models.BotCommands.EnterRace.TimeHigh;
			}
			set
			{
				Models.BotCommands.EnterRace.TimeHigh = value;
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

		public int selectedColor
		{
			get
			{
				return Models.BotCommands.EnterRace.cSelect;
			}
			set
			{

				Models.BotCommands.EnterRace.cSelect = value;
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
