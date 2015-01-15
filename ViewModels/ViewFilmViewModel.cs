using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using VideoFileRenamer.DAL;
using VideoFileRenamer.Download;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.ViewModels
{
	public class ViewFilmViewModel : ViewFilmModelViewBase
	{
		private Film film;

		public Film Film
		{
			get { return film; }
			set
			{
				if (Equals(value, film)) return;
				film = value;
				NotifyPropertyChanged();
			}
		}

		public ViewFilmViewModel(Film filme)
		{
			Film = filme;
			SelectedFilmIndex = 0;
			Query = Film.Name + " " + Film.OriginalName + " " + Film.Year;
			RefreshAsync.Execute();
		}

		#region Overrides of ViewFilmModelViewBase

		public override ActionCommand SelectAsync
		{
			get
			{
				return new ActionCommand(x =>
				{
					InternetDownloader downloader = new InternetDownloader();

					var detail = downloader.FullInfoFilm(CurrentItem[SelectedFilmIndex].Link, new PlugDownload());
					AppEngine.Create().UpdateFilm(Film, detail);
					Close.Execute();
				});
			}
		}

		public override ActionCommand Refresh
		{
			get
			{
				return new ActionCommand(x =>
				{
					var downloader = new InternetDownloader();
					CurrentItem = new ParsFilmList(null, downloader.FindFilms(Query));
				});
			}
		}

		#endregion
	}
}
