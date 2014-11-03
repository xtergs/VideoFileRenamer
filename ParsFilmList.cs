using System.Collections.Generic;
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
