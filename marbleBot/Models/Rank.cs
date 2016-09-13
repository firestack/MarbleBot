using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace marbleBot.Models
{
	public class Rank : ViewModels.Utils.ViewModel
	{
		public string username { get; set; }
		public int position { get; set; }
		public int points { get; set; }
		public int total { get { return totalStatic; } set { totalStatic = value; } }
		public static int totalStatic { get; set; }

		public class RankSortPosition : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				return Compare((Rank)x, (Rank)y);
			}

			public int Compare(Rank x, Rank y)
			{
				return x.position - y.position;
			}
		}

		public class RankSortPoints : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				return Compare((Rank)x, (Rank)y);
			}

			public int Compare(Rank x, Rank y)
			{
				return x.points - y.points;
			}
		}
	}


}
