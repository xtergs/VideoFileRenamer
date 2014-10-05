using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.DAL
{
	class FilmGenericRepository:GenericRepository<Film>
	{
		public FilmGenericRepository(FilmContext context) : base(context)
		{
		}

		public ICollection<Film> Films { get { return context.Films.ToList(); } } 
	}
}
