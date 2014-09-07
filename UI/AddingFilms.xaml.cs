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

		private ObservableCollection<ListOfParsFilms> obs;
		private ListOfParsFilms current;

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var engine = AppEngine.Create();
			obs = new ObservableCollection<ListOfParsFilms>(engine.NewFilms);
			if (engine.NewFilms.Count == 0)
			{
				this.Close();
				return;
			}
			current = engine.NewFilms.Dequeue();
			MainGrid.DataContext = current;
		}

		void SelectFilm()
		{
			AppEngine engine = AppEngine.Create();
			if (engine.NewFilms.Count == 0)
			{
				this.Close();
				return;
			}
			current = engine.NewFilms.Dequeue();
			MainGrid.DataContext = current;
		}

		private void NextButton_Click(object sender, RoutedEventArgs e)
		{
			SelectFilm();
		}
	}
}
