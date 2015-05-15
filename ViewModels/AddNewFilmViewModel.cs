using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoFileRenamer.DAL;
using VideoFileRenamer.Download;

namespace VideoFileRenamer.ViewModels
{
	public class AddNewFilmViewModel : ViewFilmModelViewBase
	{
		private readonly FilmContext context;

		public AddNewFilmViewModel(string connectionString = "FilmContext")
		{
			if (NewFiles.Count <= 0)
			{
				Close.Execute();
				return;
			}
			context = new FilmContext(connectionString);
			SelectedFilmIndex = -1;
			CurrentItem = NewFiles[0];
		}

		public int CountLast
		{
			get { return context.NewFiles.Count(); }
		}

		public ObservableCollection<ParsFilmList> NewFiles
		{
			get { return AppEngine.Create().NewFilms; }
		}

		async public void Select()
		{
			
				InternetDownloader downloader = new InternetDownloader();
				int tempSelectedindex = SelectedFilmIndex;
				var tempCurItem = CurrentItem;
				SelectedFilmIndex = -1;
				NewFiles.RemoveAt(0);
			if (NewFiles.Count != 0)
			{
				
				CurrentItem = NewFiles[0];
			}
				var detail = await downloader.FullInfoFilmAsync(tempCurItem[tempSelectedindex].Link, new PlugDownload());
				AppEngine.Create().AddNewFilm(tempCurItem.FileInfo, detail);
			if (NewFiles.Count == 0)
				Close.Execute();
		}

		public override ActionCommand SelectAsync
		{
			get
			{
				return new ActionCommand(x => Select(), o => SelectedFilmIndex >= 0);
			}
		}

		public override ActionCommand Skip
		{
			get
			{
				return new ActionCommand(x =>
				{
					SelectedFilmIndex = -1;
					NewFiles.RemoveAt(0);
					if (NewFiles.Count > 0)
						CurrentItem = NewFiles[0];
					else
					{
						Close.Execute();
					}
				});
			}
		}


		public ActionCommand DeleteFile
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		
	}
}
