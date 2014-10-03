using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	class Director
	{
		public Director()
		{
			this.Films = new HashSet<Film>();
		}

		public int IdDirector { get; set; }
		public string FistName { get; set; }
		public string SecondName { get; set; }
		public string Link { get; set; }

		public virtual ICollection<Film> Films { get; set; }
	}
}
