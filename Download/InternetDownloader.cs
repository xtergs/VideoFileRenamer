using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls.Primitives;
using HtmlAgilityPack;

namespace VideoFileRenamer.Download
{
	class InternetDownloader
	{
		Stream Response(string link)
		{
			System.Net.Http.HttpClient client = new HttpClient();
			//var response = new HttpRequestMessage(HttpMethod.Get, link);
			client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(@"Chrome", @"35.0.1916.114"));
			client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue(@"text/html"));
			client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("ru"));
			
			//client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue());

			var responseStream = client.GetStreamAsync(link);
			responseStream.Wait();
			client.Dispose();
			return responseStream.Result;
			//return responseStream;
			// создание запроса
//			HttpWebRequest myHttpWebRequest =
//				(HttpWebRequest)HttpWebRequest.Create(link);
//			// Инициализация
//			myHttpWebRequest.UserAgent = @"Mozila/4.0 (compatible; MSIE 6.0; 
//							  Windows NT 5.1; SV1; MyIE2;";
//			myHttpWebRequest.Accept = @"text/html";
//			myHttpWebRequest.Headers.Add("Accept-Language", "ru");

//			// Ответ
//			HttpWebResponse myHttpWebResponse =
//				(HttpWebResponse)myHttpWebRequest.GetResponse();
//			return myHttpWebResponse;
		}


		public List<FileVideoDetailShort> FindFilms(FileVideoInfo videoInfo)
		{
			string link = "http://www.kinopoisk.ru/index.php?kp_query=" + videoInfo.ToString();
			var myHttpWebResponse = Response(link);
			HtmlDocument document = new HtmlDocument();
			//var stream = myHttpWebResponse.GetResponseStream();
			document.Load(Response(link));

			List<FileVideoDetailShort> list = new List<FileVideoDetailShort>();
			var ss = document.DocumentNode.SelectNodes(@"//html//body//*//div[@class='search_results']//div//div[@class='info']");

			//Парсинг html
			for (int i = 0; i < ss.Count; i++)
			{
				var info = new FileVideoDetailShort();

				//Original name
				info.OriginalName = HttpUtility.HtmlDecode(ss[i].SelectSingleNode(ss[i].XPath + @"/span").InnerText);
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
			var httpResponse = Response(link);

			HtmlDocument document = new HtmlDocument();
			//var stream = httpResponse.GetResponseStream();
			document.Load(Response(link));

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
			 WebClient client = new WebClient();
			string guid = "cach\\"+Guid.NewGuid().ToString() + ".jpeg";
			returnDetail.Image = guid;
			if (!Directory.Exists("cach"))
				Directory.CreateDirectory("cach");
			client.DownloadFile( node.Attributes["src"].Value.ToString(), guid);

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
				returnDetail.Rate = (int)double.Parse(node.InnerText.Replace('.',','));

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
			var task = Task.Factory.StartNew(()=> FullInfoFilm(link, plugin));
			return task;
		}
	}
}
