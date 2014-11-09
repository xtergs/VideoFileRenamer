using System;
using System.Collections.Generic;
using System.Windows.Documents.Serialization;
using VideoFileRenamer.Annotations;

namespace VideoFileRenamer.Download
{
	[Serializable()]
	public class ParsFilmList : List<FileVideoDetailShort>
	{
	    public ParsFilmList([NotNull] FileVideoInfo fileInfo,[NotNull] IEnumerable<FileVideoDetailShort> listFilms )
		{
			FileInfo = fileInfo;
			this.AddRange(listFilms);
		}

		public FileVideoInfo FileInfo { get; set; }
	}
}
