using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	class File
	{
		public int FileID { get; set; }
		public string MD5 { get; set; }
		public string FileName { get; set; }
		public long Size { get; set; }
		public string Path { get; set; }
		public System.TimeSpan Created { get; set; }
		public System.TimeSpan Modified { get; set; }

		public virtual Film Film { get; set; }
	}
}
