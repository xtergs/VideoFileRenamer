using System.IO;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.DAL
{
	class FilmRepository : GenericRepository<Film>
	{
		public FilmRepository(FilmContext context)
			: base(context)
		{
		}

		public override void Delete(Film entityToDelete)
		{
			if (System.IO.File.Exists(entityToDelete.Image))
				System.IO.File.Delete(entityToDelete.Image);
			base.Delete(entityToDelete);
		}
	}
}
