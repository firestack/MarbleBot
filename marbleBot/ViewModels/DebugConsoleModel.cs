using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using System.Runtime.InteropServices;

namespace marbleBot.ViewModels
{
	class DebugConsoleModel : Utils.ViewModel
	{
		[DllImport("Kernel32")]
		public static extern void AllocConsole();

		[DllImport("Kernel32")]
		public static extern void FreeConsole();

		public ICommand ShowConsole
		{
			get
			{
				return GetProperty() as Utils.ViewCommand ??
					SetProperty(
						new Utils.ViewCommand(
							(obj) => AllocConsole()
						)
					);
			}
		}

		public ICommand HideConsole
		{
			get
			{
				return GetProperty() as Utils.ViewCommand ??
					SetProperty(
						new Utils.ViewCommand(
							(obj) => FreeConsole()
						)
					);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			AllocConsole();
			Console.WriteLine("test");
		}
	}
}
