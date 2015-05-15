using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VideoFileRenamer.DAL;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.ViewModels
{
	using VideoFileRenamer;
	public class FilmsViewModel:ViewModel
	{
#if DEBUG
		private static string defString = "FilmContext";
#else
		private static string defString = "default";
#endif

		#region Properties

		public ObservableCollection<Film> Films
		{
			get { return films; }
			private set
			{
				films = value;
				NotifyPropertyChanged();
			}
		}

		public ObservableCollection<Genre> Genries { get; private set; }
		public ObservableCollection<Country> Countries { get; private set; }

		public int SelectedFilmIndex
		{
			get { return selectedFilmIndex; }
			set
			{
				selectedFilmIndex = value;
				NotifyPropertyChanged("SelectedFilm");
				NotifyPropertyChanged();
			}
		}

		public Film SelectedFilm
		{
			get
			{
				if (SelectedFilmIndex >= 0 && SelectedFilmIndex < Films.Count)
					return Films[SelectedFilmIndex];
				else
					return null;
			}
		}

		public int? Year
		{
			get
			{
				if (year == 0)
					return null;
				return year;
			}
			set
			{
				if (value == null)
					year = 0;
				else
					year = (int)value;
				NotifyPropertyChanged();
			}
		}

		public string Filter
		{
			get { return filter; }
			set
			{
				filter = value;
				NotifyPropertyChanged();
			}
		}

		public int SelectedGenriesIndex
		{
			get { return selectedGenriesIndex; }
			set
			{
				selectedGenriesIndex = value;
				NotifyPropertyChanged();
			}
		}

		public Genre SelectedGenre
		{
			get
			{
				if (SelectedGenriesIndex < 0)
					return null;
				return Genries[SelectedGenriesIndex];
			}
		}

		public int SelectedCountriesIndex
		{
			get { return selectedCountriesIndex; }
			set
			{
				selectedCountriesIndex = value;
				NotifyPropertyChanged();
			}
		}

		public Country SelectedCountry
		{
			get
			{
				if (SelectedCountriesIndex < 0)
					return null;
				return Countries[SelectedCountriesIndex];
			}
		}

		#endregion

		#region constructors
		public FilmsViewModel ()
			: this(defString)
		{
			
		}

		public FilmsViewModel(string connectionString)
		{
			context = new FilmContext(connectionString);
			Films = new ObservableCollection<Film>(context.Films);
			Genries = new ObservableCollection<Genre>(context.Genres);
			Countries = new ObservableCollection<Country>(context.Countries);

			SelectedGenriesIndex = -1;
			selectedCountriesIndex = -1;
			Filter = "";
		}

		#endregion

		#region members

		private int selectedGenriesIndex;
		private int selectedCountriesIndex;
		private FilmContext context;
		private int year;
		private ObservableCollection<Film> films;
		private string filter;
		private int selectedFilmIndex;

		#endregion

		#region Overrides of ViewModel

		protected override string OnValidate(string propertyName)
		{
			return base.OnValidate(propertyName);
		}

		#endregion

		#region Commands

		public ActionCommand ClearGenre
		{
				get { return new ActionCommand(x =>
				{
					SelectedGenriesIndex = -1;
				}, o => SelectedGenriesIndex != -1); }
		}

		public ActionCommand ClearCountry
		{
			get
			{
				return new ActionCommand(x =>
				{
					SelectedCountriesIndex = -1;
				}, o => SelectedCountriesIndex != -1);
			}
		}

		public ActionCommand AdditionFilter
		{
			get
			{
				return new ActionCommand(a =>
				{
					Films = new ObservableCollection<Film>(context.Films.Where(x => x.Genres.Any(d=>d.GenreID == SelectedGenre.GenreID ) ));
				}, p=> SelectedCountriesIndex != -1 || selectedGenriesIndex != -1);
			}
		}

		public ActionCommand ClearDB
		{
			get
			{
				return new ActionCommand(a =>
				{
					if (context.Database.Exists())
						context.Database.Delete();
					context.Database.CreateIfNotExists();
				});
			}
		}

		public ActionCommand FilterCommand
		{
			get
			{
				return new ActionCommand(a =>
				{
					Films = new ObservableCollection<Film>(context.Films.Where(x=>x.Name.Contains(Filter) || x.OriginalName.Contains(Filter)));
				});
			}
		}

		#endregion
	}
}
