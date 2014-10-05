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
using VideoFileRenamer.Annotations;
using VideoFileRenamer.Download;
using VideoFileRenamer.Download.Download.Properties;
using VideoFileRenamer.Models;
using VideoFileRenamer.Properties;
using VideoFileRenamer.DAL;
using File = VideoFileRenamer.Models.File;

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

		public delegate void updatedData();

		public event statusMessage ChangedStatus;
		public event updatedData UpdatedData;

		protected virtual void OnUpdatedData()
		{
			updatedData handler = UpdatedData;
			if (handler != null) handler();
		}

		protected virtual void OnChangedStatus(string message)
		{
			statusMessage handler = ChangedStatus;
			if (handler != null) handler(message);
		}

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
			{
				current = new AppEngine();
				UnitOfWork.ConnectionString = Settings.Default.VideosConnectionString;
			}
			return current;
		}

		#endregion


		#region FindNewVideos
		//Возвращает список новых фильмов и серий для сериалов
		public Queue<FileVideoInfo> FindNewVideos()
		{
			UnitOfWork videosEntities = new UnitOfWork();
			var paths = (StringCollection)Settings.Default[Dirs];
			foreach (var path in paths)
			{

				foreach (var file in Directory.EnumerateFiles(path, "*.mkv"))
				{
					FileInfo infoFile = new FileInfo(file);
					if (!ignoringFiles.Contains(infoFile.Name) && !videosEntities.FileRepository.IsContain(infoFile))
						NewFiles.Enqueue(new FileVideoInfo(infoFile));
				}
				foreach (var file in Directory.EnumerateFiles(path, "*.avi"))
				{
					FileInfo infoFile = new FileInfo(file);
					if (!ignoringFiles.Contains(infoFile.Name) && !videosEntities.FileRepository.IsContain(infoFile))
						NewFiles.Enqueue(new FileVideoInfo(infoFile));
				}
			}

			ChangedStatus("Find " + NewFiles.Count + " films");
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
				//Исчет фильмы для файла и скачивает картинку
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
			ChangedStatus("Found films for all files");
		}

		public Task FindFilmsForAllFilesAsync()
		{
			var result = Task.Factory.StartNew(FindFilmsForAllFiles);
			return result;
		}

		public void AddNewFilm(FileVideoInfo info, FileVideoDetail detail)
		{
			using (UnitOfWork entities = new UnitOfWork())
			{
				entities.AddNewFilm(info, detail);
			}
			ChangedStatus(detail.Name + " is added");
			UpdatedData();
		}

		public void RenameAllFiles()
		{
			UnitOfWork entity = new UnitOfWork();
			foreach (var file in entity.FileRepository.dbSet)
			{
				var newName = Rename(file);
				file.FileName = newName;
			}
			entity.Save();
			entity.Dispose();
			ChangedStatus("All files were renamed");
		}

		private string Rename([NotNull] File file)
		{
			string pat = (string) Settings.Default[pattern];
			StringBuilder builder = new StringBuilder(pat);
			builder.Replace("%T", file.Film.Name);				//%T - name
			builder.Replace("%O", file.Film.OriginalName);		//%O - original name
			builder.Replace("%Y", file.Film.Year.ToString());	//%Y - Year
			if (file.Film.Genres.FirstOrDefault() != null)
			{
				builder.Replace("%G", file.Film.Genres.First().Name); //%G - Genres
			}

			builder.Append(Path.GetExtension(file.FileName)).Replace(":","");
			var newName = builder.ToString();
			System.IO.File.Move(Path.Combine(file.Path,file.FileName), Path.Combine(file.Path,newName));

			return newName;
		}

		public List<File> GetFiles(int indexFilm)
		{
			UnitOfWork entity = new UnitOfWork();
			var d =  entity.FileRepository.Get((file) => file.Film.FilmID == indexFilm).ToList();
			entity.Dispose();
			return d;
		}

		public void DeleteFile(int idFile, bool isRealFile)
		{
			UnitOfWork entity = new UnitOfWork();

			var file = entity.FileRepository.Get(x => x.FileID == idFile).First();
			if (file.Film.Files.Count == 1)
				file.Film.Deleted = true;

			if (isRealFile)
				System.IO.File.Delete(Path.Combine(file.Path, file.FileName));
			entity.FileRepository.Delete(file);
			entity.Save();
			entity.Dispose();
			ChangedStatus("The file was deleted");
		}
	}
}
