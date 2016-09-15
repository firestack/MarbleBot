using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace marbleBot.Models.BotCommands
{
	[TwitchBot.Attributes.Command(accessLevel = TwitchBot.Message.EPermissions.ALL, respondsTo = TwitchBot.Message.ECommand.PRIVMSG, prefix = '!', suffix = "")]
	class LogCommands : TwitchBot.Classes.Plugin
	{
		public static object _lock = new object();

		public static System.Collections.ObjectModel.ObservableCollection<TwitchBot.Message.Message> commands = new System.Collections.ObjectModel.ObservableCollection<TwitchBot.Message.Message>();

		public override void Init()
		{
			base.Init();
			System.Windows.Data.BindingOperations.EnableCollectionSynchronization(commands, _lock);
		}
		public override void Invoke()
		{
			commands.Add(message);
		}
	}
}
