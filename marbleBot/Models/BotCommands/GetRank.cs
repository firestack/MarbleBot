using System;
using System.Text.RegularExpressions;
using TwitchBot.Message;

namespace marbleBot.Models.BotCommands
{
	[TwitchBot.Attributes.Command(accessLevel = TwitchBot.Message.EPermissions.MOD, respondsTo = ECommand.PRIVMSG)]
	public class GetRank : TwitchBot.Classes.Plugin
	{

		static public Regex exp = new Regex(@"(\w+) is rank (\d+) of (\d+) with (\d+)", RegexOptions.Compiled);
		public Match match;

		public delegate void NewRank(Rank rank);
		public static event NewRank OnNewRank;


		public override void Invoke()
		{
			if(OnNewRank != null)
			{
				Rank.totalStatic = Convert.ToInt32(match.Groups[3].Value);
				OnNewRank(
					new Rank
					{
						username = match.Groups[1].Value,
						position = Convert.ToInt32(match.Groups[2].Value),
						points = Convert.ToInt32(match.Groups[4].Value)
					}
				);
			}
		
		}

		public override bool CanExecute(Message msg, string val)
		{
			match = exp.Match(msg.message);
			return match.Success;
		}
	}
}
