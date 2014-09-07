using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace VideoFileRenamer
{
	class ListOfParsFilms
	{
	    public List<FileVideoDetailShort> list = new List<FileVideoDetailShort>();
		public FileVideoInfo file;

		public ListOfParsFilms(FileVideoInfo fileInfo, List<FileVideoDetailShort> listFilms )
		{
			file = fileInfo;
			list = listFilms;
		}

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
