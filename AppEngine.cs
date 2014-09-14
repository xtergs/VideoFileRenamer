using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using VideoFileRenamer.Download;

namespace VideoFileRenamer.Download
{
	class AppEngine
	{

		//Singleton
		private static AppEngine current;

		private List<string> ignoringFiles = new List<string>();
		private Queue<FileVideoInfo> newFiles = new Queue<FileVideoInfo>();
		private Queue<ListOfParsFilms> newFilms = new Queue<ListOfParsFilms>(); 

		public Queue<FileVideoInfo> NewFiles
		{
			get { return newFiles; }
		}

		public Queue<ListOfParsFilms> NewFilms
		{
			get { return newFilms; }
		}

		#region Constructors

		private AppEngine()
		{
			var entity = new VideosEntities();
			entity.Films.Create();
		}

		
		public static AppEngine Create()
		{
			if (current == null)
				current = new AppEngine();
			return current;
		}

		#endregion

		//Возвращает список новых фильмов и серий для сериалов
		public Queue<FileVideoInfo> FindNewVideos(string path)
		{
			VideosEntities videosEntities = new VideosEntities();
			
			foreach (var file in Directory.EnumerateFiles(path, "*.mkv"))
			{
				FileInfo infoFile = new FileInfo(file);
				if (!ignoringFiles.Contains(infoFile.Name) && !videosEntities.Films.Any(film => film.FileName == infoFile.Name))
					newFiles.Enqueue(new FileVideoInfo(infoFile));
			}
			foreach (var file in Directory.EnumerateFiles(path, "*.avi"))
			{
				FileInfo infoFile = new FileInfo(file);
				if (!ignoringFiles.Contains(infoFile.Name) && !videosEntities.Films.Any(film => film.FileName == infoFile.Name))
					newFiles.Enqueue(new FileVideoInfo(infoFile));
			}
			return newFiles;
		}

		public Task<Queue<FileVideoInfo>> FindNewVideosAsync(string path)
		{
			var result = Task<Queue<FileVideoInfo>>.Factory.StartNew(() => FindNewVideos(path));
			return result;
		}

		//Подсчитывает хэши файлов
		public void CalculateHashFiles(List<FileVideoInfo> list)
		{
			if (list == null) 
				throw new ArgumentNullException("list in AppEngine.CalculateHashFiles");

			foreach (var fileVideoInfo in list)
			{
				fileVideoInfo.CalculateHash();
			}
		}

		//Поиск фильма на кинопоиске
		public List<FileVideoDetailShort> FindFilms(FileVideoInfo detail, PlugDownload plugin)
		{
			InternetDownloader downloader = new InternetDownloader();
			return downloader.FindFilms(detail);
		}

		//
		private FileVideoDetail DownloadInfoFilm(FileVideoDetailShort detail, PlugDownload plugin)
		{
			InternetDownloader downloader = new InternetDownloader();
			return downloader.FullInfoFilm(detail.Link, plugin);
		}

		private ListOfParsFilms FindFilmInternet(FileVideoInfo info)
		{
			InternetDownloader downloader = new InternetDownloader();
			return new ListOfParsFilms(info, downloader.FindFilms(info));
		}

		public void FindFilmsForAllFiles()
		{
			InternetDownloader downloader = new InternetDownloader();
			Parallel.ForEach(newFiles, (file) =>
			{
				var temp = FindFilmInternet(file);
				WebClient client = new WebClient();
				foreach (var item in temp.List)
				{
					if (item.Image == null)
						continue;
					string imageName = @"cach\" + Guid.NewGuid().ToString() + ".jpeg";
					client.DownloadFile(item.Image, imageName);
					item.Image = imageName;
				}
				newFilms.Enqueue(temp);
			});
		}

		public Director AddDirector(Person director)
		{
			VideosEntities entities = new VideosEntities();
			var dir = entities.Directors.Add(new Director() {FistName = director.FirstName, SecondName = director.LastName, Link = "213"});
			entities.SaveChanges();
			return dir;
		}

		public void AddNewFilm(FileVideoInfo info, FileVideoDetail detail)
		{
			VideosEntities entities = new VideosEntities();
			//info.CalculateHash();
			if (detail.DirectorId < 0)
				throw new Exception("Haven't director for film");
			var film = new Film()
			{
				Director_id = detail.DirectorId,
				FileName = info.NameFile,
				Name = detail.Name,
				OriginalName = detail.OriginalName,
				Year = detail.Year,
				Description = detail.Description,
				MD5 = info.Md5,
				Genres = AddGenres(detail.GenreList),
				Link = detail.Link,
				Countries = AddCountries(detail.CountryList),
				Rate = detail.Rate,
				Image = detail.Image
			};
		//	film.Countries = AddCountries(detail.CountryList, film);
			entities.Films.Add(film);
			entities.SaveChanges();
		}

		private ICollection<Genre> AddGenres(List<string> genreList, bool save = true)
		{
			VideosEntities entities = new VideosEntities();
			var genres = new List<Genre>();
			foreach (var genre in genreList)
			{
				if (entities.Genres.Any(gnr => gnr.Genre1 == genre))
				{
					genres.Add(entities.Genres.First(gnr => gnr.Genre1 == genre));
				}
				else
				{
					genres.Add(entities.Genres.Add(new Genre(){Genre1 = genre.Trim()}));
				}
			}
			if (save)
				entities.SaveChanges();
			entities.Dispose();
			return genres;
		}


		private ICollection<Country> AddCountries(List<string> list, bool save = true)
		{
			VideosEntities entities = new VideosEntities();
			var countrs = new List<Country>();
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = list[i].Trim('\n', ' ');
				var temp = list[i];

				//if (entities.Countries.Any(country => country.Name == temp))
				//{
				//	var d = entities.Countries.First(country => country.Name == temp);
				//	if (d != null)
				//		countrs.Add(d);
				//}
				//else
				{
					var country = new Country() {Name = list[i]};
					countrs.Add(entities.Countries.Add(country));
				}
			}
			if (save)
				entities.SaveChanges();
			entities.Dispose();
			return countrs;
		}
	}
}
