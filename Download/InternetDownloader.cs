﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using VideoFileRenamer.Models;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace VideoFileRenamer.Download
{
	class InternetDownloader
	{
		async Task<Stream> Response(string link)
		{
			System.Net.Http.HttpClient client = new HttpClient();
			//var response = new HttpRequestMessage(HttpMethod.Get, link);
			client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(@"Chrome", @"35.0.1916.114"));
			client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue(@"text/html"));
			client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("ru"));
			
			Stream responseStream;
			responseStream = await client.GetStreamAsync(link);
			return responseStream;
		}

		public List<FileVideoDetailShort> FindFilms(FileVideoInfo videoInfo)
		{
			return FindFilms(videoInfo.ToString());
		}
		
		public List<FileVideoDetailShort> FindFilms(string query)
		{

			string link = "http://www.kinopoisk.ru/index.php?kp_query=" + query;
			//var myHttpWebResponse = Response(link);
			HtmlDocument document = new HtmlDocument();
			//var stream = myHttpWebResponse.GetResponseStream();
			try
			{

				var stream = Response(link);
				if (stream.Status == TaskStatus.Faulted)
					return new List<FileVideoDetailShort>();
				document.Load(stream.Result);
			}
			catch (System.IO.IOException e)
			{
				return null;
			}
			catch (AggregateException e)
			{
				return new List<FileVideoDetailShort>();
			}
			finally
			{

			}

			List<FileVideoDetailShort> list = new List<FileVideoDetailShort>();
			var ss = document.DocumentNode.SelectNodes(@"//html//body//*//div[@class='search_results']//div//div[@class='info']");
			if (ss == null)
				return null;

			StringBuilder builder;

			//Парсинг html
			for (int i = 0; i < ss.Count; i++)
			{
				var info = new FileVideoDetailShort();

				//Original name
				//builder = new StringBuilder(HttpUtility.HtmlDecode(ss[i].SelectSingleNode(ss[i].XPath + @"/span").InnerText));
				
				info.OriginalName = HttpUtility.HtmlDecode(ss[i].SelectSingleNode(ss[i].XPath + @"/span").InnerText);
				info.OriginalName = Regex.Replace(info.OriginalName, @"([, 0-9]+мин$)*([][!""#$%&'()*,./:;<=>?@\^_`{|}~-])*", "",
					RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
				//Name
				info.Name = HttpUtility.HtmlDecode(ss[i].SelectSingleNode(ss[i].XPath + @"/p/a").InnerText);
				var temp = ss[i].SelectSingleNode(ss[i].XPath + @"/p/span");
				if (temp != null)
				{
					//Year
					int result;
					if (int.TryParse(ss[i].SelectSingleNode(ss[i].XPath + @"/p/span").InnerText, out result))
						info.Year = result;
				}
				//Link
				info.Link = @"http://www.kinopoisk.ru/" + ss[i].SelectSingleNode(ss[i].XPath + @"//p//a/@href").Attributes[0].Value;

				//Image
				var node = document.DocumentNode.SelectSingleNode(ss[i].XPath + @"/..//p[@class='pic']/a/img");
				if (node != null)
					info.Image = @"http://www.kinopoisk.ru/" + node.Attributes[2].Value;

				list.Add(info);
			}

			return list;
		}

		

		public FileVideoDetail FullInfoFilm(string link, PlugDownload plugin)
		{
			FileVideoDetail returnDetail = new FileVideoDetail();
			//var httpResponse = Response(link);

			HtmlDocument document = new HtmlDocument();
			//var stream = httpResponse.GetResponseStream();
			var stream = Response(link);
			if (stream.IsCanceled == true)
				return null;
			document.Load(stream.Result);

			var ss = document.DocumentNode.SelectSingleNode(plugin.Link);

			//Name
			returnDetail.Name = HttpUtility.HtmlDecode(ss.SelectSingleNode(ss.XPath + plugin.Name).InnerText);

			//Original Name
			returnDetail.OriginalName = HttpUtility.HtmlDecode(ss.SelectSingleNode(ss.XPath + plugin.OriginalName).InnerText);

			//Yer
			var node = ss.SelectSingleNode( plugin.Year);
			returnDetail.Year = int.Parse(node.InnerText.Trim(' ', '\n'));

			//Link
			returnDetail.Link = link;

			//image
			node = document.DocumentNode.SelectSingleNode(plugin.Image);
			using (WebClient client = new WebClient())
			{
				string guid = PlugDownload.PathImage + "cach\\" + Guid.NewGuid().ToString() + ".jpeg";
				returnDetail.Image = guid;
				if (!Directory.Exists("cach"))
					Directory.CreateDirectory("cach");
				if (node != null)
					client.DownloadFile(node.Attributes["src"].Value.ToString(), guid);
			}

			//Director
			node = document.DocumentNode.SelectSingleNode(plugin.Director);
			returnDetail.Director = new Person(node.InnerText);

			//Description
			node = document.DocumentNode.SelectSingleNode(plugin.Description);
			if (node != null)
				returnDetail.Description = HttpUtility.HtmlDecode(node.InnerText);

			//Rate
			node = document.DocumentNode.SelectSingleNode(plugin.Rating);
			if (node != null)
				returnDetail.Rate = double.Parse(node.InnerText.Replace('.',','));

			//Countres
			node = document.DocumentNode.SelectSingleNode(plugin.CountryList);
			returnDetail.CountryList = node.InnerText.Split(',').ToList();

			//Genres
			node = document.DocumentNode.SelectSingleNode(plugin.GenreList);
			returnDetail.GenreList = node.InnerText.Split(',').ToList();



			return returnDetail;
		}

		public Task<FileVideoDetail> FullInfoFilmAsync(string link, PlugDownload plugin)
		{
			var task = Task.Run(()=> FullInfoFilm(link, plugin));
			return task;
		}
	}
}
