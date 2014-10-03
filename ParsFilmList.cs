using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using VideoFileRenamer.Annotations;

namespace VideoFileRenamer.Download
{
	class ParsFilmList : List<FileVideoDetailShort>
	{
	    public ParsFilmList([NotNull] FileVideoInfo fileInfo,[NotNull] IEnumerable<FileVideoDetailShort> listFilms )
		{
			FileInfo = fileInfo;
			this.AddRange(listFilms);
		}

		public FileVideoInfo FileInfo { get; set; }
	}
}
