using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace marbleBot.Models
{
	class Bot
	{
		public TwitchBot.Classes.Bot bot;

		public Task botTask;

		public TwitchBot.Classes.Credentials cred { get; set; }

		public Bot()
		{
			bot = new TwitchBot.Classes.Bot
			{
				superusers = new HashSet<string> { "bomb_mask" },
				server = "irc.chat.twitch.tv",
				port = 80,
				fastLength = 1.0f,
				slowLength = 1.67f
			};
			var PP = bot.EH.operators.Find(obj => obj.Item1.GetType() == typeof(TwitchBot.Plugins.PrettyPrint));
			
		}

		public void StartBot(TwitchBot.Classes.Credentials cred)
		{
			bot.running = true;

			this.cred = cred;
			
			botTask = Task.Run(() => bot.Start(this.cred));
			
			
		}

		public void StopBot()
		{
			bot.Disconnect();
		}
	}
}
