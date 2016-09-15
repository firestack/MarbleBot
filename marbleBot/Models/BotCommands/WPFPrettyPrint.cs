using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace marbleBot.Models.BotCommands
{
	[TwitchBot.Attributes.Command(accessLevel = TwitchBot.Message.EPermissions.ALL, respondsTo = TwitchBot.Message.ECommand.ALL)]
	class WPFPrettyPrint : TwitchBot.Classes.Plugin
	{
		[NonSerialized]
		public System.Collections.ObjectModel.ObservableCollection<string> twitchChat = new System.Collections.ObjectModel.ObservableCollection<string>();

		public override void Init()
		{
			if (bot is TwitchBot.Classes.Bot)
			{
				(bot as TwitchBot.Classes.Bot).OnSend += Read;
			}
		}

		public void Read(string msg)
		{
			twitchChat.Add(">> " + msg);
		}

		public override void Invoke()
		{
			twitchChat.Add("<< " + message.ToString());
		}

		public override bool CanExecute(TwitchBot.Message.Message msg, string val)
		{
			return true;
		}
	}
}
