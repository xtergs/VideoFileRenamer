using System.IO;
using System.Windows.Forms;

namespace VideoFileRenamer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

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

		public string PrevFileName { get; set; }

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

		public string SizeString
		{
			get
			{
				double size = Size;
				int prefi = 0;
				while (size >= 1024)
				{
					size /= 1024;
					prefi++;
				}
				string prefix = " B";
				switch (prefi)
				{
					case 0:
						prefix = " B";
						break;
					case 1:
						prefix = " KB";
						break;
					case 2:
						prefix = " MB";
						break;
					case 3:
						prefix = " GB";
						break;
					case 4: prefix = " TB";
						break;
					case 5:
						prefix = " PB";
						break;
				}
				return size.ToString("F2") + prefix;
			}
		}
    }
}
