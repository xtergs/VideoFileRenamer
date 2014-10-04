using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	class Country
	{
		public Country()
		{
			this.Film = new HashSet<Film>();
		}

		public int CountryID { get; set; }
		public string Name { get; set; }

		public virtual ICollection<Film> Film { get; set; }
	}
}
