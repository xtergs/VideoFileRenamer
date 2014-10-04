using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	class Actor
	{
		public Actor()
		{
			this.Film = new HashSet<Film>();
		}

		public int ActorID { get; set; }
		public string FistName { get; set; }
		public string SecondName { get; set; }

		public virtual ICollection<Film> Film { get; set; }
	}
}
