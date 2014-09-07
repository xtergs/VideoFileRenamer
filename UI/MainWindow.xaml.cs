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

		private string path = @"D:\films";

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			VideosEntities entities = new VideosEntities();
			entities.Films.Add(new Film() {Name = "sdfsdf", OriginalName = "324565634", Director = new Director(){FistName = "dfsdf", SecondName = "dsf"}, Director_id = 0, Year = 2014, FileName = "df32.mkv"});
			entities.SaveChanges();
			var engine = AppEngine.Create();
			var list = await engine.FindNewVideosAsync(path);
			List<ListOfParsFilms> listOfFilms = new List<ListOfParsFilms>();
			PlugDownload plugin = new PlugDownload();

			Parallel.ForEach(list, info =>
			{
				var item = new ListOfParsFilms();
				item.list = engine.FindFilms(info, plugin);
				item.file = info;
				listOfFilms.Add(item);
			});
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
