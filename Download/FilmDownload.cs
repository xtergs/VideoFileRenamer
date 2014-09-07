﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Download
{
	class PlugDownload
	{
		private string originalLink = @"http://www.kinopoisk.ru/";
		private string link = @"//html//body//*//div[@id='photoInfoTable']";

		private string name = @"/div[@id='headerFilm']/h1";
		private string originalName = @"/div[@id='headerFilm']/span";
		private string year = @"/html[1]/body[1]/div[4]/div[1]/div[1]/div[1]/div[3]/table[1]/tr[1]/td[2]";
		private string countryList = @"/div[@infoTable]/table/tbody/tr[2]/td/div/a";
		private string director = @"/html[1]/body[1]/div[4]/div[1]/div[1]/div[1]/div[3]/table[1]/tr[4]";
		private string scenario;
		private string Operator;
		private string genreList = @"/div[@infoTable]/table/tbody/tr[11]/td/span/a";
		private string rating;
		private string image = @"//*/a[@class='popupBigImage']/img";
		private string trailer;
		private string time;

		public string Link
		{
			get { return link; }
			set { link = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string OriginalName
		{
			get { return originalName; }
			set { originalName = value; }
		}

		public string Year
		{
			get { return year; }
			set { year = value; }
		}

		public string CountryList
		{
			get { return countryList; }
			set { countryList = value; }
		}

		public string Director
		{
			get { return director; }
			set { director = value; }
		}

		public string Scenario
		{
			get { return scenario; }
			set { scenario = value; }
		}

		public string Operator1
		{
			get { return Operator; }
			set { Operator = value; }
		}

		public string GenreList
		{
			get { return genreList; }
			set { genreList = value; }
		}

		public string Rating
		{
			get { return rating; }
			set { rating = value; }
		}

		public string Image
		{
			get { return image; }
			set { image = value; }
		}

		public string Trailer
		{
			get { return trailer; }
			set { trailer = value; }
		}

		public string Time
		{
			get { return time; }
			set { time = value; }
		}
	}
}