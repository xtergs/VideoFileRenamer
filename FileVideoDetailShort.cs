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
		private int year;
		private string link;
		private string image;
		private bool isAdd = true;

		public string Image
		{
			get { return image; }
			set { image = value; }
		}
		public bool IsAdd
		{
			get { return isAdd; }
			set { isAdd = value; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		} 

		public string OriginalName
		{
			get { return originalName; }
			set { originalName = value; }
		}

		public int Year
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
