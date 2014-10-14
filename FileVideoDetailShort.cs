using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Download
{
	class FileVideoDetailShort
	{
		private string name;
		private string originalName;
		private bool isAdd = true;

		public string Image { get; set; }

		public bool IsAdd
		{
			get { return isAdd; }
			set { isAdd = value; }
		}

		public string Name {
			get { return name; }
			set { name = value.Trim(); }
		} 

		public string OriginalName
		{
			get { return originalName; }
			set { originalName = value.Trim(); }
		}

		public int Year { get; set; }

		public string Link { get; set; }

		public override string ToString()
		{
			return Name + "[" + originalName + "] " + "(" + Year + ")";
		}
	}
}
