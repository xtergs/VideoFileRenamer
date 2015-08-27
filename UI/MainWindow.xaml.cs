using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VideoFileRenamer.Annotations;
using VideoFileRenamer.DAL;
using VideoFileRenamer.Models;
using VideoFileRenamer.UI;
using VideoFileRenamer.ViewModels;

namespace VideoFileRenamer.Download
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private ObservableCollection<Film> collectionFilms;

		Film SelectedItem { get; set;  }
		public string Filter { get; set; }

		Tasks TaskWindow {  get;  set; }

		bool LastAdded { get; set; }

		public AppEngine Engine
		{
			get { return AppEngine.Create(); }
		}

		//public List<Genre> Genres
		//{
		//	get
		//	{
		//		using (UnitOfWork unit = new UnitOfWork())
		//		{
		//			return unit.GenresRepository.dbSet.Distinct().ToList();
		//		}
		//	}
		//}

		//public List<Country> Countries
		//{
		//	get
		//	{
		//		using (UnitOfWork unit = new UnitOfWork())
		//		{
		//			return unit.CountriesRepository.dbSet.Distinct().ToList();
		//		}
		//	}
		//}

		public MainWindow()
		{
			
			InitializeComponent();
			DataContext = new FilmsViewModel();
			//DataContext = AppEngine.Create().FilmsViewModel;
			TaskWindow = new Tasks();
			TaskWindow.Activate();
			TaskWindow.Show();
			var appEngine = AppEngine.Create();
			appEngine.ChangedStatus += AppEngineOnChangedStatus;
			appEngine.UpdatedData += AppEngineOnUpdatedData;
			appEngine.ProgressStatus += appEngine_ProgressStatus;
			Filter = "";
		}

		void appEngine_ProgressStatus(int n, int count, string message)
		{
			Dispatcher.InvokeAsync(() =>
			{
				InfoStatusBarItem.Content = n + " of " + count + " " + message;
			});
		}

		private void AppEngineOnUpdatedData()
		{
			Dispatcher.InvokeAsync(() => 
			{
				RefreshListFilms();
			});
		}

		private Task<ICollection<Film>> collectionTask;

		private async void RefreshListFilms()
		{
			OnPropertyChanged("Films");
			if (collectionTask != null && collectionTask.Status == TaskStatus.Running) ;
				//collectionTask.Dispose();

			collectionTask = (AppEngine.Create().FindFilmAsync(Filter));
			collectionTask.ContinueWith((x) =>
			{
				Dispatcher.InvokeAsync(() =>
				{
					var d = x.Result;
					if (LastAdded)
						listView.ItemsSource = d.OrderByDescending(w => w.Added);
					else
						listView.ItemsSource = x.Result;

					//Adition filter
					//GenresComboBox.ItemsSource = AppEngine.Create().;
					//countriesComboBox.ItemsSource = Countries;

				});
			});
			//collectionTask.Start();
		}

		private void AppEngineOnChangedStatus(string message)
		{
			Dispatcher.InvokeAsync(() =>
			{
				InfoStatusBarItem.Content = message;
			});
			
		}

		//private string path = @"D:\Films";
		private CancellationTokenSource tokens = new CancellationTokenSource();
		private bool run = false;
		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			//if (run)
			//	return;
			//try
			//{
			//	run = true;
				var engine = AppEngine.Create().StartSearchNewFilesAsync();
			//	await engine.FindNewVideosAsync(tokens.Token);
			//	await engine.FindFilmsForAllFilesAsync(tokens.Token);
			//}
			//catch (Exception ee)
			//{
			//	throw;
			//}
			//finally
			//{
			//	run = false;
			//}
			//	OutList.ItemsSource = listOfFilms;
		}

		private void FindFilms_Click(object sender, RoutedEventArgs e)
		{
		//	var engine = new AppEngine();
			RefreshListFilms();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			RefreshListFilms();
			//System.Windows.Data.CollectionViewSource filmContextViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("filmContextViewSource")));
			// Load data by setting the CollectionViewSource.Source property:
			// filmContextViewSource.Source = [generic data source]
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			UI.Settings	settings = new UI.Settings();
			settings.Activate();
			settings.Visibility = Visibility.Visible;
//			settings.Show();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var engine = AppEngine.Create();
			engine.RenameAllFiles();
		}

		void DeleteFilm(int index, bool realFile)
		{
			//AppEngine entity = AppEngine.Create();
			//var files = entity.GetFiles(((Film)ListFilms.SelectedItem).FilmID);
			//if (files.Count <= 0)
			//	return;
			//if (files.Count > 1)
			//{

			//}
			//else
			//	DeleteFile(files[0].FileID);
		}

		void DeleteFile(int idFile)
		{
			
		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			MenuItem item = (MenuItem) sender;

			var engine = AppEngine.Create();
			engine.DeleteFile((int)item.Tag, Properties.Settings.Default.RealFile);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		private void ListFilms_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			DeleteItem.Items.Clear();
			DeleteItem.Header = "Delete";

			var engine = AppEngine.Create();
			var files = engine.GetFiles(((Film)listView.SelectedItem).FilmID);
			if (files == null)
				return;

			foreach (var file in files)
			{
				var item = new MenuItem()
				{
					Header = file.FileName + " - " + file.Size.ToString(),
					Tag = file.FileID

				};
				item.Click += MenuItem_Click_1;
				DeleteItem.Items.Add(item);
			}
		}

		private void MenuItem_Click_2(object sender, RoutedEventArgs e)
		{
			AppEngine.Create().CleanDeletedFilms();
			RefreshListFilms();
		}

		void AditionFilter()
		{
			
			using (UnitOfWork de = new UnitOfWork())
			{
				//var query = de.FilmRepository.Get(x=>x.Deleted == false);
				//var list = query.ToList();

				//if (GenresComboBox.SelectedIndex >= 0)
				//{
				//	var genre = (Genre) GenresComboBox.SelectedItem;
				//	list = list.Where(x => x.Genres.Any(y=>y.GenreID == genre.GenreID)).ToList();
				//}

				//if (CountriesComboBox.SelectedIndex >= 0)
				//{
				//	var country = (Country)CountriesComboBox.SelectedItem;
				//	list = list.Where(x => x.Countries.Any(y => y.CountryID == country.CountryID)).ToList();
				//}

				//int result;
				//if (int.TryParse(YearTextBox.Text, out result))
				//{
				//	list = list.Where(x => x.Year == result).ToList();
				//}
				//ListFilms.ItemsSource = list;
			}
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			AditionFilter();
		}

		void ClearDB()
		{
			using (UnitOfWork unit = new UnitOfWork())
			{
				unit.ClearDB();
			}
		}

		private void MenuItem_Click_3(object sender, RoutedEventArgs e)
		{
			ClearDB();
			RefreshListFilms();
		}

		private void MenuItem_Click_4(object sender, RoutedEventArgs e)
		{
			AppEngine.Create().Backup();
		}

		private void MenuItem_Click_5(object sender, RoutedEventArgs e)
		{
			AppEngine.Create().Restore();
		}

		private void MenuItem_Click_6(object sender, RoutedEventArgs e)
		{
			AddingFilms newWindow = new AddingFilms(AppEngine.Create().NewFilmManager);
			newWindow.Visibility = Visibility.Visible;
			newWindow.Activate();
		}

		private void window_Closing(object sender, CancelEventArgs e)
		{
			TaskWindow.Close();
			AppEngine.Create().Backup();
		}

		private void MenuItem_Click_7(object sender, RoutedEventArgs e)
		{
			AppEngine.Create().CleanCache();
		}

		private void MenuItem_Click_8(object sender, RoutedEventArgs e)
		{
			//AppEngine.Create().NewFilms.Clear();
			AppEngine.Create().Backup();
		}

		//private void Button_Click_3(object sender, RoutedEventArgs e)
		//{
		//	YearTextBox.Text = ((Button) sender).Content.ToString();
		//	AditionFilter();
		//}

		private void Menu_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			
		}

		private void MenuItem_Checked(object sender, RoutedEventArgs e)
		{
			LastAdded = true;
			RefreshListFilms();
		}

		private void Button_Click_4(object sender, RoutedEventArgs e)
		{
			GenresComboBox.SelectedIndex = -1;
		}

		private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
		{
			LastAdded = false;
			RefreshListFilms();
		}

		private void MenuItem_Click_9(object sender, RoutedEventArgs e)
		{
			AppEngine.Create().UpdateAllInfoAsync();
		}

		private void ClearCountries(object sender, RoutedEventArgs e)
		{
			//CountriesComboBox.SelectedIndex = -1;
		}

		private void SomeList_3_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void Button_Click_5(object sender, RoutedEventArgs e)
		{
			//var counry = ((Country) ((Button) sender).Content);
			//var x = CountriesComboBox.Items.IndexOf(counry);
			//CountriesComboBox.SelectedValue = counry;
			//CountriesComboBox.SelectedItem = counry;
			//CountriesComboBox.IsSynchronizedWithCurrentItem = true;
		}

		private void MenuItem_Click_10(object sender, RoutedEventArgs e)
		{
			var view = new ViewFilm(((FilmsViewModel)DataContext).SelectedFilm);
			view.Show();
		}

		private void DeleteItem_OnClick(object sender, RoutedEventArgs e)
		{
			
		}

		private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			AppEngineOnUpdatedData();
		}

		private void StopSearchFilmsButtonClick(object sender, RoutedEventArgs e)
		{
			AppEngine.Create().StopSearchNewFiles();
		}
	}
}
