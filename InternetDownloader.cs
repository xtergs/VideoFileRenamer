using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VideoFileRenamer
{
	class InternetDownloader
	{
		HttpWebResponse Response(string link)
		{
			// создание запроса
			HttpWebRequest myHttpWebRequest =
				(HttpWebRequest)HttpWebRequest.Create(link);

			// Инициализация
			myHttpWebRequest.UserAgent = @"Mozila/4.0 (compatible; MSIE 6.0; 
							  Windows NT 5.1; SV1; MyIE2;";
			myHttpWebRequest.Accept = @"text/html";
			myHttpWebRequest.Headers.Add("Accept-Language", "ru");

			// Ответ
			HttpWebResponse myHttpWebResponse =
				(HttpWebResponse)myHttpWebRequest.GetResponse();
			return myHttpWebResponse;
		}

		public List<FileVideoDetailShort> FindFilms(FileVideoInfo videoInfo)
		{
			string link = "http://www.kinopoisk.ru/index.php?kp_query=" + videoInfo.ToString();
			var myHttpWebResponse = Response(link);
			HtmlDocument document = new HtmlDocument();
			var stream = myHttpWebResponse.GetResponseStream();
			document.Load(stream);

			List<FileVideoDetailShort> list = new List<FileVideoDetailShort>();
			var ss = document.DocumentNode.SelectNodes(@"//html//body//*//div[@class='search_results']//div//div[@class='info']");

			//Парсинг html
			for (int i = 0; i < ss.Count; i++)
			{
				var info = new FileVideoDetailShort();

				//Original name
				info.OriginalName = ss[i].SelectSingleNode(ss[i].XPath + @"/span").InnerText;
				//Name
				info.Name = ss[i].SelectSingleNode(ss[i].XPath + @"/p/a").InnerText;
				//Year
				info.Year = ss[i].SelectSingleNode(ss[i].XPath + @"/p/span").InnerText;
				//Link
				info.Link = ss[i].SelectSingleNode(ss[i].XPath + @"//p//a/@href").Attributes[0].Value;

				list.Add(info);
			}

			return list;
		}

		public FileVideoDetail FullInfoFilm(string link, PlugDownload plugin)
		{
			FileVideoDetail returnDetail = new FileVideoDetail();
			var httpResponse = Response(link);

			HtmlDocument document = new HtmlDocument();
			var stream = httpResponse.GetResponseStream();
			document.Load(stream);

			var ss = document.DocumentNode.SelectSingleNode(plugin.Link);

			returnDetail.Name = ss.SelectSingleNode(ss.XPath + plugin.Name).InnerText;
			returnDetail.OriginalName = ss.SelectSingleNode(ss.XPath + plugin.OriginalName).InnerText;
			returnDetail.Year = ss.SelectSingleNode(ss.XPath + plugin.Year).InnerText;
			returnDetail.Link = link;

			return returnDetail;
		}
	}
}
