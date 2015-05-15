using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	public class FileBase
	{
		private static string[] qualities = new[] { "web-dlrip", "hdrip", "bdrip" };
		public FileBase()
		{
		}

		public FileBase(FileInfo info)
		{
			FileName = info.Name;
			Size = info.Length;
			Path = info.FullName;
			Created = info.CreationTimeUtc;
			Modified = info.LastWriteTimeUtc;
			Quality = GetQuality(FileName.ToLowerInvariant());
		}

		static int GetQuality(string fileName)
		{
			for (int i = 0; i < qualities.Length; i++)
				if (fileName.Contains(qualities[i]))
					return i;
			return -1;
		}

		public string SearchName
		{
			get
			{
				StringBuilder builder = new StringBuilder(FileName.ToLowerInvariant());
				builder.Replace('.', ' ');
					builder.Replace(qualities[Quality], "");
				return builder.ToString();
			}
		}


		public string MD5 { get; set; }

		public string FileName { get; set; }

		public long Size { get; set; }

		public string Path { get; set; }

		public bool Deleted { get; set; }

		public int Quality { get; set; }

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
