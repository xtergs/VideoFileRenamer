using System;
using System.Collections.Generic;
using System.Linq;
using VideoFileRenamer.Download;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.DAL
{
	public class UnitOfWork:IDisposable
	{
		private FilmContext context;
		private FilmRepository filmsRepository;
		private GenericRepository<Person> directoryRepository;
		private FileRepository fileRepository;
		private GenericRepository<Genre> genresRepository;
		private GenericRepository<Country> countriesRepository;
		private GenericRepository<IgnorFile> ingoryFileRepository;

		public static string ConnectionString { get; set; }

		private bool disposed = false;

		public UnitOfWork()
		{
			context = new FilmContext(ConnectionString);
			context.Database.CreateIfNotExists();
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

		public GenericRepository<Country> CountriesRepository
		{
			get
			{
				if (this.countriesRepository == null)
				{
					countriesRepository = new GenericRepository<Country>(context);
				}
				return countriesRepository;
			}
		}

		public GenericRepository<IgnorFile> IngoringFileRepository
		{
			get
			{
				if (ingoryFileRepository == null)
					ingoryFileRepository = new GenericRepository<IgnorFile>(context);
				return ingoryFileRepository;
			}
		}

		public FilmRepository FilmRepository
		{
			get
			{
				if (this.filmsRepository == null)
				{
					this.filmsRepository = new FilmRepository(context);
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

		public GenericRepository<Person> DirectorRepository
		{
			get
			{
				if (this.directoryRepository == null)
				{
					directoryRepository = new GenericRepository<Person>(context);
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
			if (!context.Films.Any())
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
					Modified = info.Modified,
					Deleted = false
				};
				context.Files.Add(file);
			}
			else
				file.Deleted = false;
			return file;
		}

		Film AddFilm(FileVideoDetail detail)
		{
			var film =
				context.Films.FirstOrDefault(x => x.Name == detail.Name && x.OriginalName == detail.OriginalName && x.Year == detail.Year);
			if (film == null)
			{
				film = new Film();
				FilmExt.Update(film, detail);
				film.Countries = AddCountries(detail.CountryList, false);
				film.Genres = AddGenres(detail.GenreList, false);
				film.Director = AddDirector(detail.Director);
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

		public Person AddDirector(Person director)
		{
			var dir =
				context.Persons.FirstOrDefault(x => x.FirstName == director.FirstName
													   && x.SecondName == director.SecondName);
			if (dir == null)
			{
				dir = context.Persons.Add(new Models.Person() { FirstName = director.FirstName, SecondName = director.SecondName, Link = "213" });
				Save();
			}
			return dir;
		}

	}
}
