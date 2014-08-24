using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace VideoFileRenamer
{
	class AppEngine
	{
		List<FileInfo> FindVideo(string path)
		{
			List<FileInfo> list = new List<FileInfo>();
			var fils = Directory.EnumerateFiles(path);
			return list;
		}
	}
}
