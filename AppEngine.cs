using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents.Serialization;
using System.Xml;
using System.Xml.Serialization;
using VideoFileRenamer.Annotations;
using VideoFileRenamer.Models;
using VideoFileRenamer.Properties;
using VideoFileRenamer.DAL;
using System.Runtime.Serialization.Formatters;
using VideoFileRenamer.Models;
using File = VideoFileRenamer.Models.File;
using Timer = System.Timers.Timer;

namespace VideoFileRenamer.Download
{
	public class AppEngine
	{

		//Singleton
		private static AppEngine current;

		private Timer timer;

		public static bool AutoFind { get; set; }

		private string pathSaveNewFilms = "test";

		private List<string> ignoringFiles = new List<string>();

		public Queue<FileVideoInfo> NewFiles { get; private set; }

		public Queue<ParsFilmList> NewFilms
		{
			get
			{
				if (newFilms == null)
					newFilms = new Queue<ParsFilmList>();
				return newFilms;
			}
			private set { newFilms = value; }
		}

		public delegate void statusMessage(string message);

		public delegate void updatedData();

		public delegate void progressMessage(int n, int count, string message);

		public delegate void FindFilms();

		public event statusMessage ChangedStatus;
		public event progressMessage ProgressStatus;
		public event FindFilms FindFilmsStarted;
		public event FindFilms FindFilmsFinished;
		public event FindFilms FindFilmsCanceled;


		protected virtual void OnFindFilmsFinished()
		{
			FindFilms handler = FindFilmsFinished;
			if (handler != null) handler();
		}

		protected virtual void OnFindFilmsCanceled()
		{
			FindFilms handler = FindFilmsCanceled;
			if (handler != null) handler();
		}

		protected virtual void OnFindFilmsStarted()
		{
			FindFilms handler = FindFilmsStarted;
			if (handler != null) handler();
		}

		protected virtual void OnProgressStatus(int n, int count, string message)
		{
			progressMessage handler = ProgressStatus;
			if (handler != null) handler(n, count, message);
		}

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

		async void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			timer.Stop();
			await FindNewVideosAsync();
			await FindFilmsForAllFilesAsync();
			timer.Start();
		}

		private const string pattern = "Pattern";
		private const string dirs = "Dirs";

		public string Pattern { get { return pattern; } }
		public string Dirs { get { return dirs; } }

		#region Constructors

		private AppEngine()
		{
			Restore();
			PlugDownload.PathImage = Settings.Default.PathToImage;
			NewFiles = new Queue<FileVideoInfo>();
			timer = new Timer(10000);
			AutoFind = false;
			if (AutoFind)
			{
				timer.Elapsed += timer_Elapsed;
				timer.Start();
			}
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
			using (UnitOfWork videosEntities = new UnitOfWork())
			{
				var paths = (StringCollection) Settings.Default[Dirs];
				foreach (var path in paths)
				{
					if (!Directory.Exists(path))
						continue;
					foreach (var file in Directory.EnumerateFiles(path, "*.mkv"))
					{
						FileInfo infoFile = new FileInfo(file);
						if (!ignoringFiles.Contains(infoFile.Name) && !videosEntities.FileRepository.IsContain(infoFile) && !NewFilms.Any(x=>x.FileInfo.NameFile == infoFile.Name) && !NewFiles.Any(x=>x.NameFile == infoFile.Name))
							NewFiles.Enqueue(new FileVideoInfo(infoFile));
					}
					foreach (var file in Directory.EnumerateFiles(path, "*.avi"))
					{
						FileInfo infoFile = new FileInfo(file);
						if (!ignoringFiles.Contains(infoFile.Name) && !videosEntities.FileRepository.IsContain(infoFile) && !NewFilms.Any(x => x.FileInfo.NameFile == infoFile.Name) )
							NewFiles.Enqueue(new FileVideoInfo(infoFile));
					}
				}

				ChangedStatus("Find " + NewFiles.Count + " films");
				return NewFiles;
			}
		}

		public Task<Queue<FileVideoInfo>> FindNewVideosAsync()
		{
			var result = Task<Queue<FileVideoInfo>>.Run(() => FindNewVideos());
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
			var list = downloader.FindFilms(info);
			if (list == null)
				return null;
			return new ParsFilmList(info, list);
		}

		
		public void FindFilmsForAllFiles()
		{
			OnFindFilmsStarted();

			Directory.CreateDirectory(@"cach\");

			Parallel.ForEach(NewFiles, (file) =>
			{
				//Исчет фильмы для файла и скачивает картинку
				var temp = FindFilmInternet(file);
				if (temp == null)
					return;
				using (WebClient client = new WebClient())
				{
					foreach (var item in temp)
					{
						if (item.Image == null)
							continue;
						string imageName = @"cach\" + Guid.NewGuid().ToString() + ".jpeg";
						try
						{
							client.DownloadFile(item.Image, imageName);
						}
						catch (System.IO.IOException e)
						{
							OnChangedStatus("Not enought space for image");
						}
						catch (System.NotSupportedException e)
						{
						}
						catch (Exception e)
						{
							
						}
						item.Image = imageName;
					}
					NewFilms.Enqueue(temp);
				}
				OnProgressStatus(NewFilms.Count, NewFiles.Count, "Found films for " + temp.FileInfo.NameFile);
			});
			NewFiles.Clear();

			OnFindFilmsFinished();
			ChangedStatus("Found films for all files");
		}

		public Task FindFilmsForAllFilesAsync()
		{
			if (FindFilmsForAllFilesTask == null || FindFilmsForAllFilesTask.IsCompleted)
				FindFilmsForAllFilesTask = Task.Factory.StartNew(FindFilmsForAllFiles, new CancellationToken() {});
			return FindFilmsForAllFilesTask;
		}

		private Task FindFilmsForAllFilesTask;
		private Queue<ParsFilmList> newFilms;

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
			try
			{

				entity.FilmRepository.dbSet.Load();
				foreach (var file in entity.FileRepository.GetAllData().Include(x=>x.Film.Genres))
				{
					var newName = Rename(file);
					file.FileName = newName;
				}
				entity.Save();
			}
			finally
			{
				entity.Dispose();
			}
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

			builder.Append(Path.GetExtension(file.FileName)).Replace(":", "").Replace("?", "");
			var newName = builder.ToString();
			if (newName == file.FileName)
				return newName;
			string path = Path.Combine(file.Path, file.FileName);
			try
			{
				if (System.IO.File.Exists(path))
					System.IO.File.Move(path, Path.Combine(file.Path, newName));
			}
			catch (System.IO.IOException e)
			{
				OnChangedStatus(file.FileName + " haven't been renamed");
				newName = file.FileName;
			}
			

			return newName;
		}

		public List<File> GetFiles(int indexFilm)
		{
			using (UnitOfWork entity = new UnitOfWork())
			{
				var d = entity.FileRepository.Get((file) => file.Film.FilmID == indexFilm).ToList();
				return d;
			}
		}

		public ICollection<Film> FindFilm(string filter)
		{
			using (UnitOfWork unitOfWork = new UnitOfWork())
			{
				var query = unitOfWork.FilmRepository.dbSet.Where(x => x.Deleted == false)
					.Include(x => x.Genres)
					.Include(x => x.Countries)
					.Include(x => x.Files)
					.Include(x => x.Actors)
					.Include(x => x.Director);
				if (String.IsNullOrWhiteSpace(filter))
					return query.ToList();
				return query.Where(x => (x.Name.Contains(filter) || x.OriginalName.Contains(filter))).ToList();
			}
		}

		public Task<ICollection<Film>> FindFilmAsync(string filter)
		{
			var result = Task<ICollection<Film>>.Run(() => FindFilm(filter)	);
			return result;
		}

		public void DeleteFile(int idFile, bool isRealFile)
		{
			using (UnitOfWork entity = new UnitOfWork())
			{
				var file = entity.FileRepository.Get(x => x.FileID == idFile).First();
				if (file.Film.Files.Count == 1)
					file.Film.Deleted = true;

				if (isRealFile)
					System.IO.File.Delete(Path.Combine(file.Path, file.FileName));
				entity.FileRepository.Delete(file);
				entity.Save();
			}
			ChangedStatus("The file was deleted");
		}

		public void CleanDeletedFilms()
		{
			int deletedCount = 0;
			using (UnitOfWork unit = new UnitOfWork())
			{
				foreach (var file in unit.FileRepository.dbSet.AsParallel())
				{
					if (!System.IO.File.Exists(file.FullPath))
					{
						unit.FileRepository.Delete(file);
						deletedCount++;
					}
				}
				var temp = unit.FilmRepository.dbSet.AsParallel().Where(x => x.Files.Count == 0 && x.Deleted == false);
				foreach (var film in temp)
				{
					//if (film.Files.Count == 0)
					film.Deleted = true;
					deletedCount++;
				}
				unit.Save();
			}
			ChangedStatus(deletedCount + "films were deleted");
		}

		public void Backup()
		{
			BinaryFormatter serializer = new BinaryFormatter();
			using (Stream write = new FileStream(pathSaveNewFilms, FileMode.Create))
			{
				try
				{
					serializer.Serialize(write, NewFilms);
				}
				catch (System.IO.IOException e)
				{
					OnChangedStatus("Not enough space on the disk");
				}
			}

		}

		public void Restore()
		{
			BinaryFormatter serializer = new BinaryFormatter();
			if (System.IO.File.Exists(pathSaveNewFilms))
				using (Stream read = new FileStream(pathSaveNewFilms, FileMode.Open))
				{
					try
					{
						NewFilms = (Queue<ParsFilmList>) serializer.Deserialize(read);
						OnChangedStatus("Restored " + NewFilms.Count + " films");
					}
					catch (System.Runtime.Serialization.SerializationException e)
					{
						NewFilms = new Queue<ParsFilmList>();
						OnChangedStatus("Can't deserialize NewFilms");
					}
				}
		}

		public void CleanCache()
		{
				Parallel.ForEach(Directory.EnumerateFiles("cach"), (file) =>
				{
				//	foreach (var file in Directory.EnumerateFiles("cach"))
				//	{
			using (UnitOfWork unit = new UnitOfWork())
			{
						if (!unit.FilmRepository.dbSet.Any(x => x.Image == file))
							System.IO.File.Delete(file);
			}
				//	}
				});

		}

		public void UpdateAllInfo()
		{
			using (UnitOfWork unit = new UnitOfWork())
			{
				Parallel.ForEach(unit.FilmRepository.dbSet, (x) =>
				{
					InternetDownloader downloader = new InternetDownloader();

					FilmExt.Update(x, downloader.FullInfoFilm(x.Link, new PlugDownload()));
				});
				unit.Save();
			}
		}
	}
}
