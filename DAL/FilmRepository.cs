using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.DAL
{
	public class FilmRepository : GenericRepository<Film>
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

		#region Overrides of GenericRepository<Film>

		public override IEnumerable<Film> Get(Expression<Func<Film, bool>> filter)
		{
			var result = dbSet.Where(filter).Include(x=>x.Actors).Include(x=>x.Countries).Include(x=>x.Director)
				.Include(x=>x.Genres)
				.Include(x=>x.Files);
			result.Load();
			return result;
		}

		#endregion
	}
}
