using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer
{
	class ListOfParsFilms
	{
	    public List<FileVideoDetailShort> list;
		public FileVideoInfo file;

		public List<FileVideoDetailShort> List
		{
			get { return list; }
			set { list = value; }
		}

		public FileVideoInfo File
		{
			get { return file; }
			set { file = value; }
		}
	}
}
