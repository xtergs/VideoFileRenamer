using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
