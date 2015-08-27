using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using VideoFileRenamer.Download;
using VideoFileRenamer.Models;
using File = VideoFileRenamer.Models.File;

namespace VideoFileRenamer.DAL
{
	public class UnitOfWork:IDisposable
	{
		public readonly FilmContext Context;
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
			Context = new FilmContext(ConnectionString);
			Context.Database.CreateIfNotExists();
		}

		public void DeleteNewFile(int id)
		{
			Context.NewFiles.Remove(Context.NewFiles.First(x => x.NewFileID == id));
		}

		public void ClearDB()
		{
			if (Context.Database.Exists())
				Context.Database.Delete();
			Context.Database.Create();
		}


		public GenericRepository<Genre> GenresRepository
		{
			get
			{
				if (this.genresRepository == null)
				{
					genresRepository = new GenericRepository<Genre>(Context);
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
					countriesRepository = new GenericRepository<Country>(Context);
				}
				return countriesRepository;
			}
		}

		public GenericRepository<IgnorFile> IngoringFileRepository
		{
			get
			{
				if (ingoryFileRepository == null)
					ingoryFileRepository = new GenericRepository<IgnorFile>(Context);
				return ingoryFileRepository;
			}
		}

		public FilmRepository FilmRepository
		{
			get
			{
				if (this.filmsRepository == null)
				{
					this.filmsRepository = new FilmRepository(Context);
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
					this.fileRepository = new FileRepository(Context);
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
					directoryRepository = new GenericRepository<Person>(Context);
				}
				return directoryRepository;
			}
		}

		public void Save()
		{
			Context.SaveChanges();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					Context.Dispose();
				}
			}
			disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void UpdateFilm(Film film, FileVideoDetail detail)
		{
			FilmExt.Update(film, detail);
			film.Countries = AddCountries(detail.CountryList, false);
			film.Genres = AddGenres(detail.GenreList, false);
			film.Director = AddDirector(detail.Director);
			Save();
		}


		public void AddNewFilm(FileBase info, FileVideoDetail detail)
		{

			var film = AddFilm(detail);
			var file = AddFile(info);

			if (film.Files.FirstOrDefault(x => x == file) == null)
				film.Files.Add(file);
			if (!Context.Films.Any())
				Context.Films.Add(film);
			Save();
		}

		public void AddNewFile(int idFilm, FileBase file)
		{
			var film = FilmRepository.Get(x => x.FilmID == idFilm);
			if (film == null)
				return;
			File fl = AddFile(file);
			film.First().Files.Add(fl);
			Save();
		}



		private File AddFile(FileBase info)
		{

			var file = FileRepository.Contain(info);
			if (file == null)
			{
				file = new File(new FileInfo(info.FullPath));
				Context.Files.Add((File)file);
			}
			else
			{
				file.Deleted = false;
			}
			return (File)file;
		}

		Film AddFilm(FileVideoDetail detail)
		{
			var film =
				Context.Films.FirstOrDefault(x => x.Name == detail.Name && x.OriginalName == detail.OriginalName && x.Year == detail.Year);
			if (film == null)
			{
				film = new Film();
				FilmExt.Update(film, detail);
				film.Countries = AddCountries(detail.CountryList, false);
				film.Genres = AddGenres(detail.GenreList, false);
				film.Director = AddDirector(detail.Director);
				Context.Films.Add(film);
			}
			else
			{
				film.Deleted = false;
				film.Seend = false;
			}
			return film;
		}

		private ICollection<Genre> AddGenres(List<string> genreList, bool save = false)
		{
			var genres = new List<Genre>();
			foreach (var gnre in genreList)
			{
				var genre = gnre.Trim();
				if (Context.Genres.Any(gnr => gnr.Name == genre))
				{
					genres.Add(Context.Genres.First(gnr => gnr.Name == genre));
				}
				else
				{
					genres.Add(Context.Genres.Add(new Genre() { Name = genre }));
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
				var dd = Context.Countries.FirstOrDefault(country => country.Name == temp);
				if (dd != null)
				{
					countrs.Add(dd);
				}
				else
				{
					var country = new Country() { Name = list[i] };
					countrs.Add(Context.Countries.Add(country));
				}
			}
			if (save)
				Save();
			return countrs;
		}

		public Person AddDirector(Person director)
		{
			var dir =
				Context.Persons.FirstOrDefault(x => x.FirstName == director.FirstName
													   && x.SecondName == director.SecondName);
			if (dir == null)
			{
				dir = Context.Persons.Add(new Models.Person() { FirstName = director.FirstName, SecondName = director.SecondName, Link = "213" });
				Save();
			}
			return dir;
		}

	}
}
