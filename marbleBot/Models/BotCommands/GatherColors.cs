using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace marbleBot.Models.BotCommands
{
	[TwitchBot.Attributes.Command(respondsTo = TwitchBot.Message.ECommand.PRIVMSG, accessLevel = TwitchBot.Message.EPermissions.USER, prefix = '!', suffix = "marble")]
	[TwitchBot.Attributes.PluginEnabled(enabled = false)]
	public class GatherColors : TwitchBot.Classes.Plugin
	{
		public override void Invoke()
		{
			var command = message.message.Split(' ');
			if(command.Length > 1)
			{
				//if(!EnterRace.marbleColors.Contains(command[1]))
				//{
				//	EnterRace.marbleColors.Add(command[1]);
				//	EnterRace.UpdateMarbleList();
				//}
			}

		}
	}
}
