﻿using System;
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

		private ObservableCollection<ListOfParsFilms> obs;
		private ListOfParsFilms current;
		private ICollection<FileVideoDetailShort> agregated; 

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

			var detail = await downloader.FullInfoFilmAsync(selectedItem.Link, new PlugDownload());
			//var entity = new VideosEntities();
			//Director dir;
			//// Добавление режисера если нету в БД
			//if (!entity.Directors.Any(director => director.FistName == detail.Director.FirstName && 
			//									director.SecondName == detail.Director.LastName))
			//dir = engine.AddDirector(detail.Director);
			//else
			//{
			//	dir = entity.Directors.First(director => director.FistName == detail.Director.FirstName &&
			//									   director.SecondName == detail.Director.LastName);
			//}
			//detail.DirectorId = dir.IdDirector;
			engine.AddNewFilm(temp.File, detail);
			
			
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
