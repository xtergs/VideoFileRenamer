using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using VideoFileRenamer.Download;

namespace VideoFileRenamer.UI
{
	/// <summary>
	/// Interaction logic for AddingFilms.xaml
	/// </summary>
	public partial class AddingFilms : Window
	{
		public AddingFilms()
		{
			InitializeComponent();
		}

		private ObservableCollection<ParsFilmList> obs;
		private ParsFilmList current;
		private ICollection<FileVideoDetailShort> agregated; 

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var engine = AppEngine.Create();
			obs = new ObservableCollection<ParsFilmList>(engine.NewFilms);
			if (engine.NewFilms.Count == 0)
			{
				this.Close();
				return;
			}
			current = engine.NewFilms.Dequeue();
			MainGrid.DataContext = current;
		}

		async void SelectFilm()
		{
			AppEngine engine = AppEngine.Create();
			InternetDownloader downloader = new InternetDownloader();
			var selectedItem = (FileVideoDetailShort) SelectionFilmBox.SelectedItem;
			var temp = current;
			if (engine.NewFilms.Count == 0)
			{
				Close();
				//return;
			}
			else
			{
				current = engine.NewFilms.Dequeue();
				MainGrid.DataContext = current;
			}

			//var film = engine.IsContainFilm(selectedItem.Link);
			//if ( film == null)
			//{
				var detail = await downloader.FullInfoFilmAsync(selectedItem.Link, new PlugDownload());

				engine.AddNewFilm(temp.FileInfo, detail);
			//}
			//else
			//	film.Files.Add(engine.AddFile(temp.FileInfo));


		}

		private void NextButton_Click(object sender, RoutedEventArgs e)
		{
			SelectFilm();
		}

		private void SelectionFilmBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			SelectFilm();
		}

		void Skip()
		{
			current = AppEngine.Create().NewFilms.Dequeue();
			MainGrid.DataContext = current;
		}

		private void SkipButton_Click(object sender, RoutedEventArgs e)
		{
			Skip();
		}
	}
}
