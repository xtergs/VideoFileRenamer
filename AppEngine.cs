using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using VideoFileRenamer.Annotations;

namespace VideoFileRenamer
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
				if (!ignoringFiles.Contains(infoFile.Name) || !videosEntities.Films.Any(film => film.FileName == infoFile.Name))
					newFiles.Enqueue(new FileVideoInfo(infoFile));
			}
			foreach (var file in Directory.EnumerateFiles(path, "*.avi"))
			{
				FileInfo infoFile = new FileInfo(file);
				if (!ignoringFiles.Contains(infoFile.Name) || !videosEntities.Films.Any(film => film.FileName == infoFile.Name))
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
				newFilms.Enqueue(FindFilmInternet(file));
			});
		}

		public void AddNewFilm(FileVideoInfo info, FileVideoDetail detail)
		{
			VideosEntities entities = new VideosEntities();
			entities.Films.Add(new Film()
			{
				FileName = info.Name,
				Name = detail.Name,
				OriginalName = detail.OriginalName,
				Year = 2014
			});
			entities.SaveChanges();
		}
	}
}
