using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Message;

namespace marbleBot.Models.BotCommands
{
	[TwitchBot.Attributes.Command(accessLevel = TwitchBot.Message.EPermissions.TMI, respondsTo = TwitchBot.Message.ECommand.PRIVMSG)]
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
				OnNewRank(
					new Rank
					{
						username = match.Groups[1].Value,
						position = Convert.ToInt32(match.Groups[2].Value),
						points = Convert.ToInt32(match.Groups[4].Value)
					}
				);
				Rank.totalStatic = Convert.ToInt32(match.Groups[3].Value);
			}
		
		}

		public override bool CanExecute(Message msg, string val)
		{
			match = exp.Match(msg.message);
			return match.Success;
		}
	}



	[TwitchBot.Attributes.Command(accessLevel = EPermissions.TMI)]
	public class TopTen : TwitchBot.Classes.Plugin
	{
		Regex exp = new Regex(@"(#(\d*) (\w*) - (\d*))", RegexOptions.Compiled);

		public static event GetRank.NewRank OnNewRank;

		public override void Invoke()
		{
			if (OnNewRank == null)
				return;

			var matches = exp.Matches(message.message);

			foreach(Match match in matches)
			{
				try
				{
					OnNewRank(new Rank
					{
						username = match.Groups[3].Value,
						position = Convert.ToInt32(match.Groups[2].Value),			
						points = Convert.ToInt32(match.Groups[4].Value)
					});
				}
				catch (Exception)
				{
					
				}

			}
			
		}

		public override bool CanExecute(Message msg, string val)
		{
			return msg.message.StartsWith(@"Ranked Top 10:") && (msg.user.ToLower().Equals("nightbot") || msg.permission == EPermissions.SUPERUSER);
		}
	}

	[TwitchBot.Attributes.Command(accessLevel = TwitchBot.Message.EPermissions.TMI, respondsTo = TwitchBot.Message.ECommand.PRIVMSG)]
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
