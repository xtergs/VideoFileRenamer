using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	public class Person
	{
		public Person()
		{
			FirstName = "";
			SecondName = "";
			Films = new HashSet<Film>();
		}
		public Person(string FullName):this()
		{
			var strings = FullName.Split(' ');
			FirstName = strings[0];
			if (strings.Count() > 1)
				SecondName = strings[1];
		}

		public int PersonId { get; set; }
		public string FirstName { get; set; }

		public string SecondName { get; set; }
		public string Link { get; set; }

		public virtual ICollection<Film> Films { get; set; }

		#region Overrides of Object

		public override string ToString()
		{
			return FirstName + SecondName;
		}

		#endregion
	}
}
