using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoFileRenamer.Download;

namespace VideoFileRenamer.Models
{
	[NotMapped]
	class FilmExt : Film
	{
		static public void Update(Film film, FileVideoDetail detail)
		{
			film.Description = detail.Description;
			film.Image = detail.Image;
			film.Link = detail.Link;
			film.Name = detail.Name;
			film.OriginalName = detail.OriginalName;
			film.Year = detail.Year;
			film.Rate = detail.Rate;
		}
	}
}
