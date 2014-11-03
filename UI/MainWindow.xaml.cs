using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using VideoFileRenamer.Annotations;
using VideoFileRenamer.DAL;
using VideoFileRenamer.Models;
using VideoFileRenamer.UI;

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

		public List<Genre> Genres
		{
			get
			{
				using (UnitOfWork unit = new UnitOfWork())
				{
					return unit.GenresRepository.dbSet.Distinct().ToList();
				}
			}
		}

		public MainWindow()
		{
			
			InitializeComponent();
			var appEngine = AppEngine.Create();
			appEngine.ChangedStatus += AppEngineOnChangedStatus;
			appEngine.UpdatedData += AppEngineOnUpdatedData;
			appEngine.ProgressStatus += appEngine_ProgressStatus;
			Filter = "";
		}

		void appEngine_ProgressStatus(int n, int count, string message)
		{
			Dispatcher.Invoke(() =>
			{
				InfoStatusBarItem.Content = n + " of " + count + " " + message;
			});
		}

		private void AppEngineOnUpdatedData()
		{
			Dispatcher.Invoke(() => 
			{
				RefreshListFilms();
			});
		}

		private void RefreshListFilms()
		{
			using (UnitOfWork entities = new UnitOfWork())
			{
				collectionFilms = new ObservableCollection<Film>(AppEngine.Create().FindFilm(Filter, entities));
				ListFilms.ItemsSource = collectionFilms;
				GenresComboBox.ItemsSource = Genres;
			}
		}

		private void AppEngineOnChangedStatus(string message)
		{
			Dispatcher.Invoke(() =>
			{
				InfoStatusBarItem.Content = message;
			});
			
		}

		//private string path = @"D:\Films";

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			var engine = AppEngine.Create();
			await engine.FindNewVideosAsync();
			await engine.FindFilmsForAllFilesAsync();
			AddingFilms newWindow = new AddingFilms();
			newWindow.Visibility = Visibility.Visible;
			newWindow.Activate();
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
			AppEngine entity = AppEngine.Create();
			var files = entity.GetFiles(((Film)ListFilms.SelectedItem).FilmID);
			if (files.Count <= 0)
				return;
			if (files.Count > 1)
			{

			}
			else
				DeleteFile(files[0].FileID);
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
			var files = engine.GetFiles(((Film) ListFilms.SelectedItem).FilmID);
			if (files == null)
				return;
			
			foreach (var file in files)
			{
				var item = new MenuItem()
				{
					Header = file.FileName + " - " + file.Size.ToString(), Tag = file.FileID
				
				};
				item.Click += MenuItem_Click_1;
				DeleteItem.Items.Add(item);
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			Filter = ((TextBox) sender).Text;
			RefreshListFilms();
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
				var query = de.FilmRepository.dbSet.Where(x => x.Deleted == false);
				var list = query.ToList();

				if (GenresComboBox.SelectedIndex >= 0)
				{
					var genre = (Genre) GenresComboBox.SelectedItem;
					list = list.Where(x => x.Genres.Any(y=>y.Name == genre.Name)).ToList();
				}

				int result;
				if (int.TryParse(YearTextBox.Text, out result))
				{
					list = list.Where(x => x.Year == result).ToList();
				}
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
	}
}
