using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Download
{
	class FileVideoInfo
	{
		public FileVideoInfo(FileInfo path)
		{
			NameFile = path.Name;
			Path = path.DirectoryName;
			isShow = false;
			Sha = "";
			Md5 = "";
			Size = path.Length;
			//Created = path.CreationTime;
			//Modified = path.LastWriteTime;
		}

		private bool isShow;

		public TimeSpan Created { get; set; }

		public TimeSpan Modified { get; set; }

		public string Md5 { get; private set; }

		public string Sha { get; private set; }

		public string NameFile { get;  set; }

		public long Size { get; set; }

		public string Path { get; set; }

		public override string ToString()
		{
			return NameFile;
		}

		public void CalculateHash()
		{	
			using (FileStream fs = new FileStream(Path + "\\" + NameFile, FileMode.Open))
			using (BufferedStream bs = new BufferedStream(fs))
			{
				SHA1Managed sha1 = new SHA1Managed();
					byte[] hash = sha1.ComputeHash(bs);
					StringBuilder formatted = new StringBuilder(2 * hash.Length);
					foreach (byte b in hash)
					{
						formatted.AppendFormat("{0:X2}", b);
					}
				Sha = formatted.ToString();
				bs.Position = 0;
				MD5 md5 = new MD5CryptoServiceProvider();
				hash = md5.ComputeHash(bs);
				formatted = new StringBuilder(2 * hash.Length);
				foreach (byte b in hash)
				{
					formatted.AppendFormat("{0:X2}", b);
				}
				this.Md5 = formatted.ToString();
			}

		}

		public static string PrintByteArray(byte[] array)
		{
			string str = "";
			int i;
			for (i = 0; i < array.Length; i++)
			{
				str = (String.Format("{0:X2}", array[i]));
				if ((i % 4) == 3) str += ' ';
			}
			return str;
		}

	}
}
