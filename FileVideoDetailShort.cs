using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer
{
	class FileVideoDetailShort
	{
		private string name;
		private string originalName;
		private string year;
		private string link;

		public string Name {
			get { return name; }
			set { name = value; }
		} 

		public string OriginalName
		{
			get { return originalName; }
			set { originalName = value; }
		}

		public string Year
		{
			get { return year; }
			set { year = value; }
		}

		public string Link
		{
			get { return link; }
			set { link = value; }
		}

		public override string ToString()
		{
			return Name + "[" + originalName + "] " + "(" + Year + ")";
		}
	}
}
