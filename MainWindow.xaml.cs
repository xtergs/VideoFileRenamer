using System;
using System.Collections.Generic;
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
			var engine = AppEngine.Create();
			var list = await engine.FindNewVideosAsync(path);
			OutList.ItemsSource = list;
		}

		private void FindFilms_Click(object sender, RoutedEventArgs e)
		{
			var engine = new AppEngine();
			PlugDownload plugin = new PlugDownload();
			var listFilms = engine.FindFilms((FileVideoInfo) OutList.SelectedItem, plugin);
			OutList_Copy.ItemsSource = listFilms;
		}
	}
}
