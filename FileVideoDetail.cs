using System.Collections.Generic;
using System.Security.Cryptography;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.Download
{
	public class FileVideoDetail : FileVideoDetailShort
	{
		private int directorId = -1;
		private List<string> countryList;
		private List<string> genreList;

		public List<string> GenreList
		{
			get { return genreList; }
			set
			{
				value.ForEach(x=>x.Trim());
				genreList = value;
			}
		}

		public int Rate { get; set; }

		public SHA1 Sha { get; set; }

		public MD5 Md5 { get; set; }

		public string Description { get; set; }

		public int DirectorId
		{
			get { return directorId; }
			set { directorId = value; }
		}

		public Person Director { get; set; }

		public List<string> CountryList
		{
			get { return countryList; }
			set
			{
				value.ForEach(x => x.Trim());
				countryList = value;
			}
		}
	}
}
