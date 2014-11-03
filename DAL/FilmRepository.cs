using VideoFileRenamer.Models;

namespace VideoFileRenamer.DAL
{
	class FilmRepository : GenericRepository<Film>
	{
		public FilmRepository(FilmContext context)
			: base(context)
		{
		}

		
	}
}
