using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	class Genre
	{
		public Genre()
		{
			this.Films = new HashSet<Film>();
		}

		public int GenreID { get; set; }
		public string Genre1 { get; set; }

		public virtual ICollection<Film> Films { get; set; }
	}
}
