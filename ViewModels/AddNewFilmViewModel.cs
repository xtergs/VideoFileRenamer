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
		private readonly NewFilmsManager newFilmsManager;

		public AddNewFilmViewModel(NewFilmsManager newFilmsManagerp)
		{
			if (newFilmsManagerp == null)
				throw new ArgumentNullException();

			this.newFilmsManager = newFilmsManagerp;
			if (newFilmsManagerp.LeftNewFilms <= 0)
			{
				Close.Execute();
				return;
			}


			SelectedFilmIndex = -1;
			CurrentItem =newFilmsManager.BorrowParFilmList();
		}

		public int CountLast
		{
			get
			{
				if (newFilmsManager == null)
					return -1;
				return newFilmsManager.LeftNewFilms;
			}
		}

		//public ObservableCollection<ParsFilmList> NewFiles
		//{
		//	get { return AppEngine.Create().NewFilms; }
		//}

		async public void Select()
		{
			
				int tempSelectedindex = SelectedFilmIndex;
				var tempCurItem = CurrentItem;
				SelectedFilmIndex = -1;
				
			if (newFilmsManager.LeftNewFilms != 0)
			{

				CurrentItem = newFilmsManager.BorrowParFilmList();
				AppEngine.Create().AddNewFilmAsync(tempCurItem.FileInfo, tempCurItem[tempSelectedindex].Link);
			}
			else
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
					var oldCurrentItem = CurrentItem;
					CurrentItem = newFilmsManager.BorrowParFilmList();
					newFilmsManager.RetriveBorrowwedPar(oldCurrentItem);
					if (CurrentItem == null)
						Close.Execute();
				});
			}
		}


		public ActionCommand DeleteFile
		{
			get
			{
				return new ActionCommand(x =>
				{
					SelectedFilmIndex = -1;
					
					//CurrentItem = newFilmsManager.BorrowParFilmList();
					newFilmsManager.CompliteBorrowPar(CurrentItem);
					if (File.Exists(CurrentItem.FileInfo.FullPath))
						File.Delete(CurrentItem.FileInfo.FullPath);
					CurrentItem = newFilmsManager.BorrowParFilmList();
					if (CurrentItem == null)
						Close.Execute();
				});
			}
		}

		
	}
}
