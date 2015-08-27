using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.UI.WebControls;
using VideoFileRenamer.DAL;
using VideoFileRenamer.Download;
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
				if (value == null)
					return;
				films = new ObservableCollection<Film>(value.AsParallel().Where(x=> x.Deleted == false));
				NotifyPropertyChanged();
			}
		}

		public ObservableCollection<Genre> Genries
		{
			get { return genries; }
			private set
			{
				if (Equals(value, genries)) return;
				genries = value;
				NotifyPropertyChanged();
				NotifyPropertyChanged("SelectedGenre");
			}
		}

		public ObservableCollection<Country> Countries
		{
			get { return countries; }
			private set
			{
				if (Equals(value, countries)) return;
				countries = value;
				NotifyPropertyChanged();
				NotifyPropertyChanged("SelectedCountry");
			}
		}

		public List<int> YearList
		{
			get { return yearList; }
			private set
			{
				if (Equals(value, yearList)) return;
				yearList = value;
				NotifyPropertyChanged();
			}
		}

		public int SelectedYear
		{
			get { return selectedYear; }
			set
			{
				if (value == selectedYear) return;
				selectedYear = value;
				NotifyPropertyChanged();
				NotifyPropertyChanged("AdditionFilter");
			}
		}

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

		//public int? Year
		//{
		//	get
		//	{
		//		if (year == 0)
		//			return null;
		//		return year;
		//	}
		//	set
		//	{
		//		if (value == null)
		//			year = 0;
		//		else
		//			year = (int)value;
		//		NotifyPropertyChanged();
		//	}
		//}

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

		public void RefreshListFilms()
		{
			Films = new ObservableCollection<Film>(context.Films);
			Genries = new ObservableCollection<Genre>(context.Genres);
			Countries = new ObservableCollection<Country>(context.Countries);
			YearList = new List<int>(context.Films.Where(x=> !x.Deleted).Select(x=> x.Year)).Distinct().ToList();
			YearList.Add(0);
		}

		public FilmsViewModel(string connectionString)
		{
			context = new FilmContext(connectionString);
			RefreshListFilms();
			

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
		private List<int> yearList;
		private ObservableCollection<Country> countries;
		private ObservableCollection<Genre> genries;
		private int selectedYear;

		#endregion

		#region Overrides of ViewModel

		protected override string OnValidate(string propertyName)
		{
			return base.OnValidate(propertyName);
		}

		#endregion

		#region Commands

		public ActionCommand SetYear
		{
			get
			{
				return new ActionCommand(x=> SelectedYear=(int)x );
			}
		}

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

		public ActionCommand ClearYear
		{
			get
			{
				return new ActionCommand(x =>
				{
					SelectedYear = 0;
				}, o => SelectedYear != 0);
			}
		}

		public ActionCommand AdditionFilter
		{
			get
			{
				return new ActionCommand(a =>
				{
					FilterCommand.Execute();
					var res = Films.ToList();
					if (SelectedGenre != null)
						res = res.Where(x => x.Genres.Any(g => g.GenreID == SelectedGenre.GenreID)).ToList();
					if (SelectedCountry != null)
						res = res.Where(x => x.Countries.Any(c => c.CountryID == SelectedCountry.CountryID)).ToList();
					if (SelectedYear != 0)
						res = res.Where(x => x.Year == SelectedYear).ToList();
					Films = new ObservableCollection<Film>(res);
				} );
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

		public ActionCommand DeleteFile
		{
			get
			{
				return new ActionCommand(x =>
				{
					string str = x.ToString();
				});
			}
		}

		public ActionCommand UpdateAllInfoAsynCommand
		{
			get { return new ActionCommand(x => AppEngine.Create().UpdateAllInfoAsync()); }
		}

		public ActionCommand StartSearchNewFilesCommand
		{
			get { return new ActionCommand(x => AppEngine.Create().StartSearchNewFilesAsync()); }
		}
	}
}
