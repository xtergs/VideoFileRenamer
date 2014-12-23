using System.IO;
using System.Windows.Forms;

namespace VideoFileRenamer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

	public partial class File : FileBase
    {
		public File(FileInfo info)
	    {
		    FileName = info.Name;
		    Size = info.Length;
		    Path = info.FullName;
			Created = info.CreationTimeUtc;
			Modified = info.LastWriteTimeUtc;
			Deleted = false;
	    }

	    public File()
	    {
		    
	    }

		public int FileID { get; set; }

		public string PrevFileName { get; set; }

        public int? Film_FilmID { get; set; }

        public virtual Film Film { get; set; }

    }
}
