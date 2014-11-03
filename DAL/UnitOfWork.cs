using System;
using System.Collections.Generic;
using System.Linq;
using VideoFileRenamer.Download;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.DAL
{
	class UnitOfWork:IDisposable
	{
		private FilmContext context;
		private GenericRepository<Film> filmsRepository;
		private GenericRepository<Director> directoryRepository;
		private FileRepository fileRepository;
		private GenericRepository<Genre> genresRepository; 

		public static string ConnectionString { get; set; }

		private bool disposed = false;

		public UnitOfWork()
		{
			context = new FilmContext(ConnectionString);
		}

		public void ClearDB()
		{
			if (context.Database.Exists())
				context.Database.Delete();
			context.Database.Create();
		}


		public GenericRepository<Genre> GenresRepository
		{
			get
			{
				if (this.genresRepository == null)
				{
					genresRepository = new GenericRepository<Genre>(context);
				}
				return genresRepository;
			}
		}

		public GenericRepository<Film> FilmRepository
		{
			get
			{
				if (this.filmsRepository == null)
				{
					this.filmsRepository = new GenericRepository<Film>(context);
				}
				return filmsRepository;
			}
		}

		public FileRepository FileRepository
		{
			get
			{
				if (this.fileRepository == null)
				{
					this.fileRepository = new FileRepository(context);
				}
				return fileRepository;
			}
		}

		public GenericRepository<Director> DirectorRepository
		{
			get
			{
				if (this.directoryRepository == null)
				{
					directoryRepository = new GenericRepository<Director>(context);
				}
				return directoryRepository;
			}
		}

		public void Save()
		{
			context.SaveChanges();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					context.Dispose();
				}
			}
			disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void AddNewFilm(FileVideoInfo info, FileVideoDetail detail)
		{

			var film = AddFilm(detail);
			var file = AddFile(info);

			if (film.Files.FirstOrDefault(x => x == file) == null)
				film.Files.Add(file);
			context.Films.Add(film);
			Save();
		}

		private File AddFile(FileVideoInfo info)
		{
			var file = context.Files.FirstOrDefault(x => x.FileName == info.NameFile && x.Size == info.Size);
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
				context.Files.Add(file);
			}
			return file;
		}

		Film AddFilm(FileVideoDetail detail)
		{
			var film =
				context.Films.FirstOrDefault(x => x.Name == detail.Name && x.OriginalName == detail.OriginalName && x.Year == detail.Year);
			if (film == null)
			{
				film = new Models.Film()
				{
					Countries = AddCountries(detail.CountryList),
					Description = detail.Description,
					Director = AddDirector(detail.Director),
					Genres = AddGenres(detail.GenreList),
					Image = detail.Image,
					Link = detail.Link,
					Name = detail.Name,
					OriginalName = detail.OriginalName,
					Year = detail.Year,
					Rate = detail.Rate
				};
				context.Films.Add(film);
			}
			else
			{
				film.Deleted = false;
			}
			return film;
		}

		private ICollection<Genre> AddGenres(List<string> genreList, bool save = false)
		{
			var genres = new List<Genre>();
			foreach (var gnre in genreList)
			{
				var genre = gnre.Trim();
				if (context.Genres.Any(gnr => gnr.Name == genre))
				{
					genres.Add(context.Genres.First(gnr => gnr.Name == genre));
				}
				else
				{
					genres.Add(context.Genres.Add(new Genre() { Name = genre }));
				}
			}
			if (save)
				Save();
			return genres;
		}


		private ICollection<Country> AddCountries(List<string> list, bool save = true)
		{
			var countrs = new List<Country>();
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = list[i].Trim('\n', ' ');
				var temp = list[i];
				var dd = context.Countries.FirstOrDefault(country => country.Name == temp);
				if (dd != null)
				{
					countrs.Add(dd);
				}
				else
				{
					var country = new Country() { Name = list[i] };
					countrs.Add(context.Countries.Add(country));
				}
			}
			if (save)
				Save();
			return countrs;
		}

		public Director AddDirector(Person director)
		{
			var dir =
				context.Directors.FirstOrDefault(x => x.FistName == director.FirstName
													   && x.SecondName == director.LastName);
			if (dir == null)
			{
				dir = context.Directors.Add(new Director() { FistName = director.FirstName, SecondName = director.LastName, Link = "213" });
				Save();
			}
			return dir;
		}

	}
}
