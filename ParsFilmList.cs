using System;
using System.Collections.Generic;
using System.Windows.Documents.Serialization;
using VideoFileRenamer.Annotations;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.Download
{
	[Serializable()]
	public class ParsFilmList : List<FileVideoDetailShort>
	{
	    public ParsFilmList([NotNull] FileBase fileInfo,[NotNull] IEnumerable<FileVideoDetailShort> listFilms )
		{
			FileInfo = fileInfo;
			this.AddRange(listFilms);
		}

		public FileBase FileInfo { get; set; }
	}
}
