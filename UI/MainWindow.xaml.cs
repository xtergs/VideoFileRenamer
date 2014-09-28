using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoFileRenamer.UI;

namespace VideoFileRenamer.Download
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ObservableCollection<Film> collectionFilms;

		public MainWindow()
		{
			InitializeComponent();
			var appEngine = AppEngine.Create();
			//appEngine.ChangedStatus += AppEngineOnChangedStatus;
		}

		private void AppEngineOnChangedStatus(string message)
		{
			InfoStatusBarItem.Content = message;
		}

		private string path = @"D:\Films";

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			var engine = AppEngine.Create();
			await engine.FindNewVideosAsync(path);
			await engine.FindFilmsForAllFilesAsync();
			AddingFilms newWindow = new AddingFilms();
			newWindow.Visibility = Visibility.Visible;
			newWindow.Activate();
			//	OutList.ItemsSource = listOfFilms;
		}

		private void FindFilms_Click(object sender, RoutedEventArgs e)
		{
		//	var engine = new AppEngine();
			PlugDownload plugin = new PlugDownload();
			//var listFilms = engine.FindFilms((FileVideoInfo) OutList.SelectedItem, plugin);
			//OutList_Copy.ItemsSource = listFilms;
			VideosEntities entities = new VideosEntities();
			collectionFilms = new ObservableCollection<Film>(entities.Films);
			ListFilms.DataContext = collectionFilms;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			VideosEntities entities = new VideosEntities();
			//entities.Database.Delete();
			entities.Database.CreateIfNotExists();
			collectionFilms = new ObservableCollection<Film>(entities.Films);
			ListFilms.DataContext = collectionFilms;
		}
	}
}
