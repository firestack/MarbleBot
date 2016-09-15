using TwitchBot.Message;

namespace marbleBot.Models.BotCommands
{
	[TwitchBot.Attributes.Command(accessLevel = EPermissions.TMI, respondsTo = ECommand.PRIVMSG)]
	public class RankCaller : TwitchBot.Classes.Plugin
	{
		public delegate void OnUpdate(RankCaller self);
		public static event OnUpdate Update;

		static public int shoutLimit = 500;
		static public int shoutCount = 0;

		public override void Invoke()
		{
			bot.PM(message.channel, "!rank");
			Update?.Invoke(this);
		}

		public override bool CanExecute(Message msg, string val)
		{
			shoutCount = (shoutLimit >= shoutCount) ? shoutCount + 1 : 0;
			Update?.Invoke(this);
			return !(shoutLimit >= shoutCount);
		}
	}
}
