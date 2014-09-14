using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Download
{
	class FileVideoDetail : FileVideoDetailShort
	{
		private string image;
		private Person director;
		private int directorId = -1;
		private List<string> countryList;
		private List<string> genreList; 
		private string description;
		private MD5 md5;
		private SHA1 sha;
		private int rate;

		public List<string> GenreList
		{
			get { return genreList; }
			set { genreList = value; }
		}

		public int Rate
		{
			get { return rate; }
			set { rate = value; }
		}

		public SHA1 Sha
		{
			get { return sha; }
			set { sha = value; }
		}

		public MD5 Md5
		{
			get { return md5; }
			set { md5 = value; }
		}

		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		public int DirectorId
		{
			get { return directorId; }
			set { directorId = value; }
		}
		public Person Director
		{
			get { return director; }
			set { director = value; }
		}

		public string Image
		{
			get { return image; }
			set { image = value; }
		}

		public List<string> CountryList
		{
			get { return countryList; }
			set { countryList = value; }
		}
	}
}
