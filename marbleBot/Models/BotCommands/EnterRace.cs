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

		public static List<string> marbleList = new List<string> { "darkred", "darkblue", "orange", "gold", "red", "limegreen", "flesh", "blue", "pink", "brown", "white", "black", "purple", "green", "neonpink", "yellow", "lightpurple", "teal", "lightgreen" };

		public static string cSelect = "";

		public static int totalRaces = 0;

		public static double timeHigh = 2.0f;

		public static double timeLow = 1.0f;

		public static double timeSelection = 0.0f;

		System.Timers.Timer tick = new System.Timers.Timer();

		Random rng = new Random();

		public override void Init()
		{
			marbleList.Sort();
			marbleList.Insert(0, "Random");
			marbleList.Insert(1, "Random No Color");
			base.Init();

			//UpdateMarbleList();

			//tick.Interval =
			tick.AutoReset = false;
			tick.Elapsed += (a, b) => {

				//var color = (cSelect == "Human Random"? marbleList[rng.Next(0, marbleList.Count)] : cSelect);
				var color = "";
				if(cSelect == "Random")
				{
					color = marbleList[rng.Next(0, marbleList.Count)];
				}
				else if(cSelect == "Random No Color")
				{
					color = "";
				}
				else
				{
					color = cSelect;
				}

				bot.PM(message.channel, "!marble " + color);
				totalRaces += 1;
			};

		}

		public override void Invoke()
		{
			//bot.PM(message.channel, "!marble " + marbleColor);

			tick.Interval = timeSelection = TimeSpan.FromSeconds(rng.NextDouble() * (timeHigh - timeLow) + timeLow).TotalMilliseconds;
			tick.Start();
		}

		public override bool CanExecute(Message msg, string val)
		{
			return msg.user.ToLower().Equals("nightbot") && msg.message.Equals("Enter marbles now!");
		}
	}
}
