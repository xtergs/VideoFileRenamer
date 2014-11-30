using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	using System;
    using System.ComponentModel.DataAnnotations.Schema;

	public class IgnorFile : FileBase
	{
		public IgnorFile(FileInfo info)
	    {
		    FileName = info.Name;
		    Size = info.Length;
		    Path = info.FullName;
			Created = info.CreationTimeUtc;
			Modified = info.LastWriteTimeUtc;
	    }

		public IgnorFile()
	    {
		    
	    }
		[Key]
		public int FileId { get; set; }

	}
}
