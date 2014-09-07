using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Download
{
	class FileVideoDetail : FileVideoDetailShort
	{
		private string image;
		private Person director;

		public Person Director
		{
			get { return director; }
			set { director = value; }
		}

		public string Image
		{
			get { return image; }
			set { image = value; }
		}
	}
}
