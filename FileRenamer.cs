using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideoFileRenamer
{
	class FileRenamer
	{
		public static string Rename(File file, string pattern)
		{
			StringBuilder builder = new StringBuilder(pattern);
			builder.Replace("%N", file.Film.Name);				//%N - name
			builder.Replace("%O", file.Film.OriginalName);		//%O - original name
			builder.Replace("%Y", file.Film.Year.ToString());	//%Y - Year
			builder.Replace("%G", file.Film.Genres.ToString());	//%G - Genres
			
			return builder.ToString();
		}
	}
}
