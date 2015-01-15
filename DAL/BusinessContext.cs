using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.DAL
{
	class BusinessContext:IDisposable
	{
		private readonly FilmContext context;
		private bool disposed;

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		protected virtual void Dispose(bool disposable)
		{
			if (disposed || !disposable)
				return;

			if (context != null)
				context.Dispose();

			disposed = true;
		}
	}
}
