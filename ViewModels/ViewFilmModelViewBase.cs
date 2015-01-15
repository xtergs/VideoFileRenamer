using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoFileRenamer.DAL;
using VideoFileRenamer.Download;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.ViewModels
{
	public class ViewFilmModelViewBase : ViewModel
	{
		public  readonly FilmContext context;
		private ParsFilmList currentItem;
		private int selectedFilmIndex;
		private string query;

		public ViewFilmModelViewBase(string connectionString = "default")
		{
			context = new FilmContext(connectionString);
		}

		public string Query
		{
			get
			{
				if (String.IsNullOrWhiteSpace(query))
					Query = Path.GetFileNameWithoutExtension(CurrentItem.FileInfo.FileName);
				return query;
			}

			set
			{
				query = value;
				NotifyPropertyChanged();
			}
		}

		public ParsFilmList CurrentItem
		{
			get { return currentItem; }
			set
			{
				currentItem = value;
				Query = Path.GetFileNameWithoutExtension(currentItem.FileInfo.FileName);
				NotifyPropertyChanged();
				NotifyPropertyChanged("Query");
			}
		}

		public int SelectedFilmIndex
		{
			get { return selectedFilmIndex; }
			set
			{
				selectedFilmIndex = value;
				NotifyPropertyChanged();
			}
		}

		public virtual ActionCommand SelectAsync
		{
			get
			{
				return new ActionCommand(x=>{});
			}
		}

		public virtual ActionCommand Skip
		{
			get
			{
				return new ActionCommand(x =>
				{
					
				});
			}
		}

		public virtual ActionCommand Refresh
		{
			get
			{
				return new ActionCommand(x =>
				{
					var downloader = new InternetDownloader();
					CurrentItem = new ParsFilmList(CurrentItem.FileInfo, downloader.FindFilms(Query));
				});
			}
		}

		public virtual ActionCommand RefreshAsync
		{
			get
			{
				return new ActionCommand(x => RefreshAsyncF());
			}
		}

		async void RefreshAsyncF()
		{
			await Task.Run(new Action(Refresh.Execute));
		}

		public ActionCommand Close
		{
			get
			{
				return new ActionCommand(x =>
				{
					context.Dispose();
					OnRequestClose();
				});
			}
		}

		public event EventHandler RequestClose;

		protected void OnRequestClose()
		{
			EventHandler handler = this.RequestClose;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		public FileVideoDetailShort SelectedFilm
		{
			get
			{
				if (SelectedFilmIndex < 0)
					return null;
				return CurrentItem[SelectedFilmIndex];
			}
			set
			{
				if (value == null)
					SelectedFilmIndex = -1;
				else
					SelectedFilmIndex = CurrentItem.IndexOf(value);
				NotifyPropertyChanged();
			}
		}
	}
}
