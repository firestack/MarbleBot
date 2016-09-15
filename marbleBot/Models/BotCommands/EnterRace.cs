using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Message;

namespace marbleBot.Models.BotCommands
{
	[TwitchBot.Attributes.Command(accessLevel = TwitchBot.Message.EPermissions.MOD, respondsTo = TwitchBot.Message.ECommand.PRIVMSG)]
	class EnterRace : TwitchBot.Classes.Plugin
	{
		//public static HashSet<string> marbleColors = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "blue", "green" };

		public static List<string> marbleList = cfg.ProgramConfig.conf.colorOptions;

		public static int cSelect = -2;

		public static double timeSelection = 0.0f;

		System.Timers.Timer tick = new System.Timers.Timer();

		Random rng = new Random();

		public static int TotalRaces
		{
			get
			{
				return cfg.ProgramConfig.conf.totalRaces;
			}

			set
			{
				cfg.ProgramConfig.conf.totalRaces = value;
			}
		}

		public static double TimeHigh
		{
			get
			{
				return cfg.ProgramConfig.conf.timeHigh;
			}

			set
			{
				cfg.ProgramConfig.conf.timeHigh = value;
			}
		}

		public static double TimeLow
		{
			get
			{
				return cfg.ProgramConfig.conf.timeLow;
			}

			set
			{
				cfg.ProgramConfig.conf.timeLow = value;
			}
		}

		public override void Init()
		{
			base.Init();


			tick.AutoReset = false;
			tick.Elapsed += EnterMarble;

		}

		private void EnterMarble(object sender, System.Timers.ElapsedEventArgs e)
		{
			var color = "";
			switch (cSelect - 2)
			{
				case -2:
					color = marbleList[rng.Next(0, marbleList.Count)];
					break;

				case -1:
					color = "";
					break;

				default:
					var pos = Math.Max(Math.Abs(cSelect), marbleList.Count - 1) + 2;
					color = pos >= marbleList.Count ? "" : marbleList[pos];
					break;
			}

			bot.PM(message.channel, "!marble " + color);
			TotalRaces += 1;
		}

		public override void Invoke()
		{
			tick.Interval = timeSelection = TimeSpan.FromSeconds(rng.NextDouble() * (TimeHigh - TimeLow) + TimeLow).TotalMilliseconds;
			tick.Start();
		}

		public override bool CanExecute(Message msg, string val)
		{
			return msg.user.ToLower().Equals("nightbot") && msg.message.Equals("Enter marbles now!");
		}
	}
}
