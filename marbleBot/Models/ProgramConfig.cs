using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace marbleBot.cfg
{
	public class ProgramConfig : ConfigLoader.Config
	{
		protected static ProgramConfig _conf;
		public new static ProgramConfig conf
		{
			get
			{
				if(_conf == null)
				{
					try
					{
						_conf = ConfigLoader.LoadConfig<ProgramConfig>(defaultFilename);
					}
					catch (Exception)
					{
						_conf = new ProgramConfig { filename = defaultFilename };
						_conf.Save();
					}
				}
				return _conf;
			}
		}

		public static string defaultFilename = "MR.cfg.json";

		public List<string> colorOptions = new List<string>();

		public int totalRaces = 0;

		public double timeHigh = 2.0f;

		public double timeLow = 1.0f;

		public List<string> commands = new List<string>();
	}
}
