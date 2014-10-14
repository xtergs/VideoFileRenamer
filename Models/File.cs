using System.IO;

namespace VideoFileRenamer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class File
    {
		public File(FileInfo info)
	    {
		    FileName = info.Name;
		    Size = info.Length;
		    Path = info.FullName;
			Created = info.CreationTimeUtc;
			Modified = info.LastWriteTimeUtc;
	    }

	    public File()
	    {
		    
	    }

        public int FileID { get; set; }

        public string MD5 { get; set; }

        public string FileName { get; set; }

        public long Size { get; set; }

        public string Path { get; set; }

	    [NotMapped]
	    public string FullPath
	    {
		    get { return Path + System.IO.Path.DirectorySeparatorChar + FileName; }
	    }

	    public DateTime Created { get; set; }

		public DateTime Modified { get; set; }

        public int? Film_FilmID { get; set; }

        public virtual Film Film { get; set; }
    }
}
