using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	public class NewFile : FileBase
	{

		public NewFile()
			:base()
		{
		}

		public NewFile(FileInfo info)
			: base(info)
		{
			
		}
		public int NewFileID { get; set; }
	}
}
