using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace VideoFileRenamer.Download
{
	class ListOfParsFilms
	{
	    public List<Film> list = new List<Film>();
		public File file;

		public ListOfParsFilms(File fileInfo, List<Film> listFilms )
		{
			file = fileInfo;
			list = listFilms;
		}

		public List<Film> List
		{
			get { return list; }
			set { list = value; }
		}

		public File File
		{
			get { return file; }
			set { file = value; }
		}
	}
}
