using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
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
			DataContext = AppEngine.Create().AddNewFilmViewModel;
		}

		private Queue<string> deleteImage; 

		//private int foundCount;
		//private int allCount;


		//private ObservableCollection<ParsFilmList> obs;
		//private ParsFilmList current;
		//private ICollection<FileVideoDetailShort> agregated; 

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

		}

		//void RefindListFilms(string query)
		//{
		//	var downloader = new InternetDownloader();
		//	current = new ParsFilmList(current.FileInfo, downloader.FindFilms(query));
		//}

		private void NextButton_Click(object sender, RoutedEventArgs e)
		{
			AppEngine.Create().AddNewFilmViewModel.SelectAsync.Execute();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		private void AddingWindow_Closed(object sender, System.EventArgs e)
		{
			if (deleteImage != null)
			foreach (var image in deleteImage)
			{
				if (File.Exists(image))
					File.Delete(image);
			}
		}

		//private async void Button_Click(object sender, RoutedEventArgs e)
		//{
		//	Cursor = Cursors.Wait;
		//	var tmp = QuerySearch.Text;
		//	await Task.Run(()=>
		//		RefindListFilms(tmp));
		//	Cursor = Cursors.Arrow;
		//	MainGrid.DataContext = current;
		//}
	}
}