using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using marbleBot.ViewModels.Utils;
using System.Deployment.Internal;
namespace marbleBot.ViewModels
{
	class InfoViewModel : ViewModel
	{
		public string versionNumber
		{
			get
			{
				return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public IEnumerable<string> extraCommands
		{
			get
			{
				return cfg.ProgramConfig.conf.commands;
			}
		}
	}
}
