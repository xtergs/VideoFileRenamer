using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer
{
	class Person
	{
		private string firstName;
		private string lastName;

		public Person(string FullName)
		{
			var strings = FullName.Split(' ');
			firstName = strings[0];
			if (strings.Count() > 0)
				lastName = strings[1];
		}

		public string FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		}

		public string LastName
		{
			get { return lastName; }
			set { lastName = value; }
		}
	}
}
