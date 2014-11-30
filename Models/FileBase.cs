using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	public class FileBase
	{



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
