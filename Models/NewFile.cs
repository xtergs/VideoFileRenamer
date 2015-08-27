using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	[Serializable]
	public class NewFile : FileBase
	{

		protected NewFile()
			
		{
		}

		public NewFile(FileInfo info)
			: base(info)
		{
			
		}
		public int NewFileID { get; set; }
	}
}
