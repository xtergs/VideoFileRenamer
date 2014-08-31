using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer
{
	class FileVideoInfo
	{
		public FileVideoInfo(FileInfo path)
		{
			name = path.Name;
			this.path = path.FullName;
			isShow = false;
			sha = "";
			md5 = "";
		}

		private string name;
		private string path;
		private bool isShow;
		private string sha;
		private string md5;

		public override string ToString()
		{
			return name;
		}

		public void CalculateHash()
		{
			using (FileStream fs = new FileStream(path, FileMode.Open))
			using (BufferedStream bs = new BufferedStream(fs))
			{
				SHA1Managed sha1 = new SHA1Managed();
					byte[] hash = sha1.ComputeHash(bs);
					StringBuilder formatted = new StringBuilder(2 * hash.Length);
					foreach (byte b in hash)
					{
						formatted.AppendFormat("{0:X2}", b);
					}
				sha = formatted.ToString();
				bs.Position = 0;
				MD5 md5 = new MD5CryptoServiceProvider();
				hash = md5.ComputeHash(bs);
				formatted = new StringBuilder(2 * hash.Length);
				foreach (byte b in hash)
				{
					formatted.AppendFormat("{0:X2}", b);
				}
				this.md5 = formatted.ToString();
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
