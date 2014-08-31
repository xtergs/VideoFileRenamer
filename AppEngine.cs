using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
		List<string> storedFiles = new List<string>(); //BD

		public static AppEngine Create()
		{
			if (current == null)
				current = new AppEngine();
			return current;
		}

		//Возвращает список новых фильмов и серий для сериалов
		public List<FileVideoInfo> FindNewVideos(string path)
		{
			List<FileVideoInfo> list = new List<FileVideoInfo>();
			foreach (var file in Directory.EnumerateFiles(path, "*.mkv"))
			{
				FileInfo infoFile = new FileInfo(file);
				if (!ignoringFiles.Contains(infoFile.Name) || !storedFiles.Contains(infoFile.Name))
					list.Add(new FileVideoInfo(infoFile));
			}
			foreach (var file in Directory.EnumerateFiles(path, "*.avi"))
			{
				FileInfo infoFile = new FileInfo(file);
				if (!ignoringFiles.Contains(infoFile.Name) || !storedFiles.Contains(infoFile.Name))
					list.Add(new FileVideoInfo(infoFile));
			}
			return list;
		}

		public Task<List<FileVideoInfo>> FindNewVideosAsync(string path)
		{
			var result = Task<List<FileVideoInfo>>.Factory.StartNew(() =>
			{
				return FindNewVideos(path); 
				
			});
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
		public FileVideoDetail DownloadInfoFilm(FileVideoDetailShort detail, PlugDownload plugin)
		{
			InternetDownloader downloader = new InternetDownloader();
			return downloader.FullInfoFilm(detail.Link, plugin);
		}
	}
}
