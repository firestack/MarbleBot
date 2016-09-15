using System;
using System.Text.RegularExpressions;
using TwitchBot.Message;

namespace marbleBot.Models.BotCommands
{
	[TwitchBot.Attributes.Command(accessLevel = EPermissions.MOD)]
	public class TopTen : TwitchBot.Classes.Plugin
	{
		Regex exp = new Regex(@"(#(\d*) (\w*) - (\d*))", RegexOptions.Compiled);

		public static event GetRank.NewRank OnNewRank;

		public override void Invoke()
		{
			if (OnNewRank == null)
				return;

			var matches = exp.Matches(message.message);

			foreach (Match match in matches)
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
}
