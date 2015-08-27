using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoFileRenamer.Download;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.DAL
{
	
	public class NewFilmsManager
	{
		struct VisorBorrowedPar
		{
			private readonly ParsFilmList par;
			private readonly DateTime timeShtamp;

			public VisorBorrowedPar(ParsFilmList parr) : this()
			{
				this.par = parr;
				timeShtamp = DateTime.UtcNow;
			}

			public ParsFilmList Par
			{
				get { return par; }
			}

			public DateTime TimeShtamp
			{
				get { return timeShtamp; }
			}
		}

		private List<VisorBorrowedPar> borrowedPars = new List<VisorBorrowedPar>();
		private Queue<ParsFilmList> newFilms;

		private Timer borrowedParCollectorTimer;
		private byte borrowParCollectMins;

		private Queue<ParsFilmList> NewFilms
		{
			get
			{
				if (newFilms == null)
					newFilms = new Queue<ParsFilmList>();
				return newFilms;
			}
			set
			{
				newFilms = value; 
			}
		}

		

		private void BorrowedParCollectorTimerElapsed(object o)
		{
			var nowDateTimeUtc = DateTime.UtcNow;
			for (int i = 0; i < borrowedPars.Count; i++)
			{
				if (nowDateTimeUtc.Subtract(borrowedPars[i].TimeShtamp).TotalMinutes > BorrowParCollectMins)
				{
					borrowedPars.Remove(borrowedPars[i]);
					NewFilms.Enqueue(borrowedPars[i].Par);
				}
			}
		}

		public NewFilmsManager()
		{
			borrowedParCollectorTimer = new Timer(BorrowedParCollectorTimerElapsed, null, new TimeSpan(0, 0, borrowParCollectMins, 0, 0), new TimeSpan(0, 0, borrowParCollectMins, 0, 0));
		}

		public byte BorrowParCollectMins
		{
			get
			{
				if (borrowParCollectMins == 0)
					borrowParCollectMins = 5;
				return borrowParCollectMins;
			}
			set
			{
				if (value == 0)
					return;
				borrowParCollectMins = value;
				borrowedParCollectorTimer.Change(new TimeSpan(0, 0, borrowParCollectMins, 0, 0),
					new TimeSpan(0, 0, borrowParCollectMins, 0, 0));
			}
		}

		public ParsFilmList BorrowParFilmList()
		{
			if (NewFilms.Count <= 0)
				return null;
			borrowedPars.Add(new VisorBorrowedPar(NewFilms.Peek()));
			return NewFilms.Dequeue();
		}

		public bool CompliteBorrowPar(ParsFilmList par)
		{
			var match =
				borrowedPars.Any(
					x =>
						x.Par == par );
			if (match)
			{
				borrowedPars.Remove(borrowedPars.First(x =>
						x.Par == par ));
				return true;
			}
			return CompliteBorrowPar(par.FileInfo);
		}

		public bool CompliteBorrowPar(FileBase par)
		{
			var match =
				borrowedPars.Any(
					x =>
						((x.Par.FileInfo.Size == par.Size) && (x.Par.FileInfo.Modified == par.Modified) &&
						 (x.Par.FileInfo.FileName == par.FileName)));
			if (match)
			{
				borrowedPars.Remove(borrowedPars.First(x =>
						
						((x.Par.FileInfo.Size == par.Size) && (x.Par.FileInfo.Modified == par.Modified) &&
						 (x.Par.FileInfo.FileName == par.FileName))));
				return true;
			}
			return false;
		}

		public void RetriveBorrowwedPar(ParsFilmList par)
		{
			var match = borrowedPars.Any(x => x.Par.FileInfo.FullPath == par.FileInfo.FullPath);
			if (match)
			{
				var foundElem = borrowedPars.First(x => x.Par.FileInfo.FullPath == par.FileInfo.FullPath);
				borrowedPars.Remove(foundElem);
				NewFilms.Enqueue(foundElem.Par);
			}
		}

		public int LeftNewFilms { get { return NewFilms.Count; } }

		public bool Contain(Predicate<ParsFilmList> predicate)
		{

			bool res = false;
			res |= borrowedPars.Any(x => predicate(x.Par));
			res |= NewFilms.Any(x => predicate(x));
			return res;
		}

		public void Add(ParsFilmList par)
		{
			NewFilms.Enqueue(par);
		}
	}
}
