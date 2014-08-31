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

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			var engine = AppEngine.Create();
			InternetDownloader downl = new InternetDownloader();
			var list = downl.FindFilms(new FileVideoInfo(new FileInfo(@"D:\Films\Джек Райан Теория хаоса [Jack Ryan Shadow Recruit] (2013) [боевик].mkv")));
			OutList.ItemsSource = list;
		}
	}
}
