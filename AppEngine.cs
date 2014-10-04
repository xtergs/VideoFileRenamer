using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using Microsoft.SqlServer.Server;
using VideoFileRenamer.Download;
using VideoFileRenamer.Download.Download.Properties;
using VideoFileRenamer.Properties;

namespace VideoFileRenamer.Download
{
	class AppEngine
	{

		//Singleton
		private static AppEngine current;

		private List<string> ignoringFiles = new List<string>();

		public Queue<FileVideoInfo> NewFiles { get; private set; }

		public Queue<ParsFilmList> NewFilms { get; private set; }

		public delegate void statusMessage(string message);

		public event statusMessage ChangedStatus;

		private const string pattern = "Pattern";
		private const string dirs = "Dirs";

		public string Pattern { get { return pattern; } }
		public string Dirs { get { return dirs; } }

		#region Constructors

		private AppEngine()
		{
			NewFilms = new Queue<ParsFilmList>();
			NewFiles = new Queue<FileVideoInfo>();
			//var entity = new VideosEntities();
			//entity.Films.Create();
		}

		
		public static AppEngine Create()
		{
			if (current == null)
				current = new AppEngine();
			return current;
		}

		#endregion


		#region FindNewVideos
		//Возвращает список новых фильмов и серий для сериалов
		public Queue<FileVideoInfo> FindNewVideos()
		{
			VideosEntities videosEntities = new VideosEntities();
			var paths = (StringCollection)Settings.Default[Dirs];
			foreach (var path in paths)
			{

				foreach (var file in Directory.EnumerateFiles(path, "*.mkv"))
				{
					FileInfo infoFile = new FileInfo(file);
					if (!ignoringFiles.Contains(infoFile.Name) && !videosEntities.Files.Any(film => film.FileName == infoFile.Name))
						NewFiles.Enqueue(new FileVideoInfo(infoFile));
				}
				foreach (var file in Directory.EnumerateFiles(path, "*.avi"))
				{
					FileInfo infoFile = new FileInfo(file);
					if (!ignoringFiles.Contains(infoFile.Name) && !videosEntities.Files.Any(film => film.FileName == infoFile.Name))
						NewFiles.Enqueue(new FileVideoInfo(infoFile));
				}
			}

			//ChangedStatus("Find " + newFiles.Count + " films");
			videosEntities.Dispose();
			return NewFiles;
		}

		public Task<Queue<FileVideoInfo>> FindNewVideosAsync()
		{
			var result = Task<Queue<FileVideoInfo>>.Factory.StartNew(() => FindNewVideos());
			return result;
		}

		#endregion 

		//Подсчитывает хэши файлов
		public void CalculateHashFiles(List<FileVideoInfo> list)
		{
			if (list == null) 
				throw new ArgumentNullException("list");

			foreach (var fileVideoInfo in list)
			{
				fileVideoInfo.CalculateHash();
			}
		}

		//Поиск фильма на кинопоиске
		//public List<FileVideoDetailShort> FindFilms(FileVideoInfo detail, PlugDownload plugin)
		//{
		//	InternetDownloader downloader = new InternetDownloader();
		//	return downloader.FindFilms(detail);
		//}

		//Поиск фильма на кинопоиске
		private FileVideoDetail DownloadInfoFilm(FileVideoDetailShort detail, FileVideoInfo info, PlugDownload plugin)
		{
			InternetDownloader downloader = new InternetDownloader();
			return downloader.FullInfoFilm(detail.Link, plugin);
		}

		//Получает список фильмов подходящих для файла
		private ParsFilmList FindFilmInternet(FileVideoInfo info)
		{
			InternetDownloader downloader = new InternetDownloader();
			return new ParsFilmList(info, downloader.FindFilms(info));
		}

		
		public void FindFilmsForAllFiles()
		{
			Directory.CreateDirectory(@"cach\");

			Parallel.ForEach(NewFiles, (file) =>
			{
				//Исчеет фильмы для файла и скачивает картинку
				var temp = FindFilmInternet(file);
				WebClient client = new WebClient();
				foreach (var item in temp)
				{
					if (item.Image == null)
						continue;
					string imageName = @"cach\" + Guid.NewGuid().ToString() + ".jpeg";
					client.DownloadFile(item.Image, imageName);
					item.Image = imageName;
				}
				NewFilms.Enqueue(temp);
				client.Dispose();
			});
		}

		public Task FindFilmsForAllFilesAsync()
		{
			var result = Task.Factory.StartNew(FindFilmsForAllFiles);
			return result;
		}

		public Director AddDirector(Person director,VideosEntities entities)
		{
			var dir =
				entities.Directors.FirstOrDefault(x => x.FistName == director.FirstName
				                                       && x.SecondName == director.LastName);
			if (dir == null)
			{
				dir = entities.Directors.Add(new Director() {FistName = director.FirstName, SecondName = director.LastName, Link = "213"});
				entities.SaveChanges();
			}
			return dir;
		}

		public void AddNewFilm(FileVideoInfo info, FileVideoDetail detail)
		{
			using (VideosEntities entities = new VideosEntities())
			{
				var film = AddFilm(detail, entities);
				var file = AddFile(info, entities);

				if (film.Files.FirstOrDefault(x => x == file) == null)
					film.Files.Add(file);
				entities.Films.Add(film);
				entities.SaveChanges();
			}
		}

		private File AddFile(FileVideoInfo info, VideosEntities entities)
		{
			var file = entities.Files.FirstOrDefault(x => x.FileName == info.NameFile && x.Size == info.Size);
			if (file == null)
			{
				file = new File()
				{
					FileName = info.NameFile,
					MD5 = info.Md5,
					Path = info.Path,
					Size = info.Size,
					Created = info.Created,
					Modified = info.Modified
				};
				entities.Files.Add(file);
			}
			return file;
		}

		public File AddFile(FileVideoInfo info)
		{
			VideosEntities entity = new VideosEntities();
			var file = AddFile(info, entity);

			entity.Dispose();

			return file;
		}

		Film AddFilm(FileVideoDetail detail, VideosEntities entities)
		{
			var film =
				entities.Films.FirstOrDefault(x => x.Name == detail.Name && x.OriginalName == detail.OriginalName && x.Year == detail.Year);
			if (film == null)
			{
				film = new Film()
				{
					Countries = AddCountries(detail.CountryList, entities),
					Description = detail.Description,
					Director = AddDirector(detail.Director,entities),
					Genres = AddGenres(detail.GenreList, entities),
					Image = detail.Image,
					Link = detail.Link,
					Name = detail.Name,
					OriginalName = detail.OriginalName,
					Year = detail.Year,
					Rate = detail.Rate
				};
				entities.Films.Add(film);
			}
			return film;
		}

		private ICollection<Genre> AddGenres(List<string> genreList, VideosEntities entities, bool save = true)
		{
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
			return genres;
		}


		private ICollection<Country> AddCountries(List<string> list, VideosEntities entities, bool save = true)
		{
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
			return countrs;
		}

		public void RenameAllFiles()
		{
			VideosEntities entity = new VideosEntities();
			foreach (var file in entity.Files)
			{
				var newName = Rename(file);
				file.FileName = newName;
			}
			entity.SaveChanges();
			entity.Dispose();
		}

		private string Rename(File file)
		{
			string pat = (string) Settings.Default[pattern];
			StringBuilder builder = new StringBuilder(pat);
			builder.Replace("%T", file.Film.Name);				//%T - name
			builder.Replace("%O", file.Film.OriginalName);		//%O - original name
			builder.Replace("%Y", file.Film.Year.ToString());	//%Y - Year
			if (file.Film.Genres.FirstOrDefault() != null)
			{
				builder.Replace("%G", file.Film.Genres.First().Genre1); //%G - Genres
			}

			builder.Append(Path.GetExtension(file.FileName)).Replace(":","");
			var newName = builder.ToString();
			System.IO.File.Move(Path.Combine(file.Path,file.FileName), Path.Combine(file.Path,newName));

			return newName;
		}

		public List<File> GetFiles(int indexFilm)
		{
			VideosEntities entity = new VideosEntities();
			var d =  entity.Files.Where((file) => file.Film.IdFilm == indexFilm);
			entity.Dispose();
			return d.ToList();
		}

		public void DeleteFile(int idFile, bool isRealFile)
		{
			VideosEntities entity = new VideosEntities();

			var file = entity.Files.First(x => x.IdFile == idFile);
			if (file.Film.Files.Count == 1)
				file.Film.Deleted = true;

			if (isRealFile)
				System.IO.File.Delete(Path.Combine(file.Path, file.FileName));
			entity.Files.Remove(file);
			entity.SaveChanges();
			entity.Dispose();
		}

		public Film IsContainFilm(string link)
		{
			using (VideosEntities entity = new VideosEntities())
			{
				return entity.Films.FirstOrDefault(x => x.Link == link);
			}
		}
	}
}
