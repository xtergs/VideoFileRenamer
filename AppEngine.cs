using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using MediaInfoDotNet;
using VideoFileRenamer.Annotations;
using VideoFileRenamer.DAL;
using VideoFileRenamer.Extender;
using VideoFileRenamer.Models;
using VideoFileRenamer.Properties;
using VideoFileRenamer.ViewModels;
using File = VideoFileRenamer.Models.File;
using Timer = System.Timers.Timer;

namespace VideoFileRenamer.Download
{
	public class AppEngine: INotifyPropertyChanged
	{

		//Singleton
		private static AppEngine current;

		private Timer timer;

		public static bool AutoFind { get; set; }

		private string pathSaveNewFilms = "test";

		//private List<string> ignoringFiles = new List<string>();

		//public ObservableCollection<FileVideoInfo> NewFiles { get; private set; }

		readonly NewFilmsManager newFilmManager = new NewFilmsManager();

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
			OnPropertyChanged("Films");
			updatedData handler = UpdatedData;
			if (handler != null) handler();
		}

		protected virtual void OnChangedStatus(string message)
		{
			statusMessage handler = ChangedStatus;
			if (handler != null) handler(message);
		}

		async void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			timer.Stop();
			await StartSearchNewFilesAsync();
			//timer.Start();
		}

		private const string pattern = "Pattern";
		private const string dirs = "Dirs";

		public string Pattern { get { return pattern; } }
		public string Dirs { get { return dirs; } }

		public NewFilmsManager NewFilmManager
		{
			get { return newFilmManager; }
		}

		#region Constructors

		private AppEngine()
		{
			Restore();
			PlugDownload.PathImage = Settings.Default.PathToImage;
			//NewFiles = new ObservableCollection<FileVideoInfo>();
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
				UnitOfWork.ConnectionString = "FilmContext";
			}
			return current;
		}

		#endregion

		private CancellationTokenSource tokens = new CancellationTokenSource();
		private bool run = false;
		public async Task StartSearchNewFilesAsync()
		{
			timer.Stop();
			if (run)
				return;
			try
			{
				tokens = new CancellationTokenSource();
				run = true;
				await FindNewVideosAsync(tokens.Token);
				await FindFilmsForAllFilesAsync(tokens.Token);
			}
			catch (Exception ee)
			{
				OnChangedStatus("Exception: StartSearchNewFilesAsync " + ee.Message);
				throw;
			}
			finally
			{
				run = false;
				tokens = null;
				timer.Start();
			}
		}

		public void StopSearchNewFiles()
		{
			if (tokens != null)
				tokens.Cancel();
		}

		#region FindNewVideos

		
		FileBase GetFile(UnitOfWork videosEntities, FileInfo infoFile)
		{
			File lowFileName = new File(infoFile);
			if (videosEntities.Context.IgnoringFiles.Any(x => x.SearchName.Contains(lowFileName.SearchName)) ||
			 NewFilmManager.Contain(x => x.FileInfo.SearchName.Contains(lowFileName.SearchName)) ||
				videosEntities.Context.NewFiles.Any(x => x.SearchName.Contains(lowFileName.SearchName)))
				return null;
			File returnFile = null;
			returnFile = videosEntities.FileRepository.HaveSimilarName(lowFileName);
			if (returnFile != null)
			{
				MediaFile fill = new MediaFile(infoFile.FullName);
				MediaFile ll = new MediaFile(returnFile.FullPath);
				if (!fill.Equal(ll))
				{
					File fl = new File(new FileInfo(lowFileName.FullPath));
					videosEntities.AddNewFile(returnFile.Film.FilmID, fl);
					return fl;
				}
				else
				{
					returnFile.Deleted = false;
					returnFile.Film.Deleted = false;
					return returnFile;
				}

			}
			return videosEntities.Context.NewFiles.Add(new NewFile(infoFile));
		}

		void FindNewFiles(IEnumerable<string> directories)
		{
			using (UnitOfWork videosEntities = new UnitOfWork())
			{

				foreach (var path in directories)
				{
					if (!Directory.Exists(path))
						continue;
					var files = Directory.EnumerateFiles(path, "*.mkv").ToList();
					files.AddRange(Directory.EnumerateFiles(path, "*.avi"));
					foreach (var file in files)
					{
						FileInfo infoFile = new FileInfo(file);
						GetFile(videosEntities, infoFile);
						OnUpdatedData();
					}
				}
				videosEntities.Save();
			}
		}
		//Возвращает список новых фильмов и серий для сериалов
		public void FindNewVideos()
		{
			FindNewFiles(((StringCollection) Settings.Default[Dirs]).Cast<string>());
		}

		public Task FindNewVideosAsync()
		{
			var result = Task.Run(() => FindNewVideos());
			return result;
		}

		public Task FindNewVideosAsync(CancellationToken token)
		{
			var result = Task.Run(() => FindNewVideos(), token);
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
		private ParsFilmList FindFilmInternet(NewFile info)
		{
			InternetDownloader downloader = new InternetDownloader();
			var list = downloader.FindFilms(info.SearchName);
			if (list == null)
				return null;
			return new ParsFilmList(info, list);
		}


		
		public void FindFilmsForAllFiles(CancellationToken ctoken)
		{
			OnFindFilmsStarted();

			Directory.CreateDirectory(@"cach\");

			using (var unit = new UnitOfWork())
			{
#if DEBUG
				var d = unit.Context.NewFiles;
#endif
				List<NewFile> listToRemove = new List<NewFile>();
				foreach (var file in unit.Context.NewFiles)
				
				//Parallel.ForEach(unit.Context.NewFiles, (file) =>
				{
					if (!System.IO.File.Exists(file.FullPath))
					{
						listToRemove.Add(file);
						continue;
					}
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
							catch (IOException e)
							{
								OnChangedStatus("Not enought space for image");
							}
							catch (NotSupportedException e)
							{
							}
							catch (Exception e)
							{

							}
							item.Image = imageName;
						}
					 NewFilmManager.Add(temp);
					}
					listToRemove.Add(file);
					OnProgressStatus(NewFilmManager.LeftNewFilms, unit.Context.NewFiles.Count(), "Found films for " + temp.FileInfo.FileName);
					if (ctoken.IsCancellationRequested)
					{
						ChangedStatus("Search films of file has been cancelled");
						break;
					}
				};
				unit.Context.NewFiles.RemoveRange(listToRemove);
				unit.Save();
			}

			OnFindFilmsFinished();
			ChangedStatus("Found films for all files");
		}

		//public Task FindFilmsForAllFilesAsync()
		//{
		//	if (FindFilmsForAllFilesTask == null || FindFilmsForAllFilesTask.IsCompleted)
		//		FindFilmsForAllFilesTask = Task.Factory.StartNew(FindFilmsForAllFiles, new CancellationToken() {});
		//	return FindFilmsForAllFilesTask;
		//}

		public Task FindFilmsForAllFilesAsync(CancellationToken token)
		{
			if (FindFilmsForAllFilesTask == null || FindFilmsForAllFilesTask.IsCompleted)
				FindFilmsForAllFilesTask = Task.Run(()=>FindFilmsForAllFiles(token), token);
			return FindFilmsForAllFilesTask;
		}

		private Task FindFilmsForAllFilesTask;
		private ObservableCollection<ParsFilmList> newFilms;
		private FilmsViewModel filmsViewModel;
		private AddNewFilmViewModel addNewFilmViewModel;
		private ViewFilmViewModel viewFilmViewModel;

		public void AddNewFilm(FileBase info, FileVideoDetail detail)
		{
			if (!NewFilmManager.CompliteBorrowPar(info))
			{
				OnChangedStatus(detail.Name + " has expired!");
				return;
			}
			using (UnitOfWork entities = new UnitOfWork())
			{
				entities.AddNewFilm(info, detail);
			}
			OnChangedStatus(detail.Name + " has added");
			OnUpdatedData();
		}

		public void AddNewFilm(FileBase info, string linkFullData)
		{
			InternetDownloader download = new InternetDownloader();
			try
			{
				var detail = download.FullInfoFilm(linkFullData, new PlugDownload());
				AddNewFilm(info, detail);
			}
			catch (Exception e)
			{
				//TODO get Logger
				throw;
			}
		}

		public Task AddNewFilmAsync(FileBase info, string linkFullData)
		{
			//Action act = () => AddNewFilm(info, linkFullData);
			return Task.Run(()=> AddNewFilm(info, linkFullData));
		}

		public void UpdateFilm(Film film, FileVideoDetail detail)
		{
			using (UnitOfWork unit = new UnitOfWork())
			{
				unit.UpdateFilm(film, detail);
			}
			OnChangedStatus(detail.Name + " has updated");
			OnUpdatedData();
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
					//file.PrevFileName = file.FileName;
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
			builder.Replace("%AQ", file.AutiodQuality);			//%AQ - autioQuality
			builder.Replace("%VQ", file.Quality);				//%VQ - vidioQuality
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
				string newFullPath = Path.Combine(file.Path, newName);
				if (System.IO.File.Exists(newFullPath))
					newFullPath = Path.Combine(file.Path, Path.GetFileNameWithoutExtension(newName) + "_2" + Path.GetExtension(newName));
				if (System.IO.File.Exists(path))
				{
					System.IO.File.Move(path, newFullPath);
				}
			}
			catch (IOException e)
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

		public Task<ICollection<Film>> FindFilmAsync(string filter, CancellationToken token)
		{
			var result = Task<ICollection<Film>>.Run(() => FindFilm(filter), token);
			return result;
		}

		public void DeleteFile(int idFile, bool isRealFile)
		{
			using (UnitOfWork entity = new UnitOfWork())
			{
				var file = entity.FileRepository.Get(x => x.FileID == idFile).First();
				//if (file.Film.Files.Count == 1)
				//	file.Film.Deleted = true;

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
					//TODO develop a backup mechanizm of newFilmManager
					//serializer.Serialize(write, NewFilms);
				}
				catch (IOException e)
				{
					OnChangedStatus("Not enough space on the disk");
				}
			}

		}

		public void Restore()
		{
			//TODO develop a restore mechanizm of newFilmManager
			//BinaryFormatter serializer = new BinaryFormatter();
			//if (System.IO.File.Exists(pathSaveNewFilms))
			//	using (Stream read = new FileStream(pathSaveNewFilms, FileMode.Open))
			//	{
			//		try
			//		{
			//			NewFilms = (ObservableCollection<ParsFilmList>)serializer.Deserialize(read);
			//			OnChangedStatus("Restored " + NewFilms.Count + " films");
			//		}
			//		catch (System.Runtime.Serialization.SerializationException e)
			//		{
			//			NewFilms = new ObservableCollection<ParsFilmList>();
			//			OnChangedStatus("Can't deserialize NewFilms");
			//		}
			//	}
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

		protected void UpdateAllInfo()
		{
			OnChangedStatus("Update of all films started");
			using (UnitOfWork unit = new UnitOfWork())
			{
				int complitedCound = 0;
				int allCount = unit.FilmRepository.dbSet.Count();
				Parallel.ForEach(unit.FilmRepository.dbSet, (x) =>
				{
					InternetDownloader downloader = new InternetDownloader();

					var temp = downloader.FullInfoFilm(x.Link, new PlugDownload());
					if (temp != null)
					{
						FilmExt.Update(x, temp);
						Interlocked.Increment(ref complitedCound);
						OnProgressStatus(complitedCound, allCount, "Updated " + temp.Name);
					}
					else
					{
						Interlocked.Increment(ref complitedCound);
						OnProgressStatus(complitedCound, allCount, "Not updated " + temp.Name);
					}
				});
				unit.Save();
			}
			OnChangedStatus("Update of all films have ended");
		}

		public Task UpdateAllInfoAsync()
		{
			return Task.Run(()=>UpdateAllInfo());
		}

		

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
