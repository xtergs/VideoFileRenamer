using System;
using System.Collections.Generic;
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

namespace VideoFileRenamer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private string path = @"D:\FilmsTest";

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			var engine = AppEngine.Create();
			engine.FindNewVideos(path);
			engine.FindFilmsForAllFiles();
			AddingFilms newWindow = new AddingFilms();
			newWindow.Visibility = Visibility.Visible;
			newWindow.Activate();
			//	OutList.ItemsSource = listOfFilms;
		}

		private void FindFilms_Click(object sender, RoutedEventArgs e)
		{
			var engine = new AppEngine();
			PlugDownload plugin = new PlugDownload();
			//var listFilms = engine.FindFilms((FileVideoInfo) OutList.SelectedItem, plugin);
			//OutList_Copy.ItemsSource = listFilms;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			VideosEntities entities = new VideosEntities();
			entities.Directors.Load();
			entities.Genres.Load();
			entities.Films.Load();
			ListFilms.DataContext = entities.Films.ToList();
		}
	}
}
