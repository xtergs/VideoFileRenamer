using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls.Primitives;
using HtmlAgilityPack;

namespace VideoFileRenamer.Download
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
				info.OriginalName = HttpUtility.HtmlDecode(ss[i].SelectSingleNode(ss[i].XPath + @"/span").InnerText);
				//Name
				info.Name = HttpUtility.HtmlDecode(ss[i].SelectSingleNode(ss[i].XPath + @"/p/a").InnerText);
				var temp = ss[i].SelectSingleNode(ss[i].XPath + @"/p/span");
				if (temp != null)
				{
					//Year
					info.Year = ss[i].SelectSingleNode(ss[i].XPath + @"/p/span").InnerText;
				}
				//Link
				info.Link = @"http://www.kinopoisk.ru/" + ss[i].SelectSingleNode(ss[i].XPath + @"//p//a/@href").Attributes[0].Value;

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

			returnDetail.Name = HttpUtility.HtmlDecode(ss.SelectSingleNode(ss.XPath + plugin.Name).InnerText);
			returnDetail.OriginalName = HttpUtility.HtmlDecode(ss.SelectSingleNode(ss.XPath + plugin.OriginalName).InnerText);
			var node = ss.SelectSingleNode( plugin.Year);
			returnDetail.Year = node.InnerText;
			returnDetail.Link = link;
			node = document.DocumentNode.SelectSingleNode(plugin.Image);
			 WebClient client = new WebClient();
			string guid = "cach\\"+Guid.NewGuid().ToString() + ".jpeg";
			returnDetail.Image = guid;
			if (!Directory.Exists("cach"))
				Directory.CreateDirectory("cach");
			client.DownloadFile(link +"//"+ node.Attributes["src"].Value.ToString(), guid);

			node = document.DocumentNode.SelectSingleNode(plugin.Director);
			returnDetail.Director = new Person(node.InnerText);
			return returnDetail;
		}
	}
}
