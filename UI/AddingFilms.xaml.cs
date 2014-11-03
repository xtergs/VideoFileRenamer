using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using VideoFileRenamer.Annotations;
using VideoFileRenamer.Download;

namespace VideoFileRenamer.UI
{
	/// <summary>
	/// Interaction logic for AddingFilms.xaml
	/// </summary>
	public partial class AddingFilms : Window, INotifyPropertyChanged
	{
		public AddingFilms()
		{
			InitializeComponent();
		}

		private int foundCount;
		private int allCount;

		public int FoundCount
		{
			get { return foundCount; }
			private set
			{
				foundCount = value;
				OnPropertyChanged();
			}
		}

		public int AllCount
		{
			get { return allCount; }
			private set
			{
				allCount = value;
				OnPropertyChanged();
			}
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
				Close();
				return;
			}
			current = engine.NewFilms.Dequeue();
			MainGrid.DataContext = current;
			AllCount = engine.NewFilms.Count + 1;
			FoundCount = 1;
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
				FoundCount++;
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
			FoundCount++;
		}

		private void SkipButton_Click(object sender, RoutedEventArgs e)
		{
			Skip();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
