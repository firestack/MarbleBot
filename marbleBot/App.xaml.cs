using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace marbleBot
{
	// TODO: Implement Colors on chat
	// TODO: Custom Command buttons 
	// TODO: Commands Popover/Flyover -> Join Lobby, nightbot commands
	// TODO: Settings Flyover

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Exit(object sender, ExitEventArgs e)
		{
			cfg.ProgramConfig.conf.Save();
		}
	}
}
