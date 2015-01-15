using System.IO;
using System.Windows.Forms;

namespace VideoFileRenamer.Models
{
	using System;
	using System.ComponentModel.DataAnnotations.Schema;

	public partial class File : FileBase
	{
		public File(FileInfo info)
			:base(info)
		{
			
			Deleted = false;
		}

		public File()
		{
			
		}

		public int FileID { get; set; }

		public string Quality { get; set; }

		public string PrevFileName { get; set; }

		public int? Film_FilmID { get; set; }

		public virtual Film Film { get; set; }

	}
}
