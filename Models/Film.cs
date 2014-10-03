using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer.Models
{
	class Film
	{
		Film()
		 {
            this.Deleted = false;
            this.Genres = new HashSet<Genre>();
            this.Countries = new HashSet<Country>();
            this.Files = new HashSet<File>();
            this.Actors = new HashSet<Actor>();
        }
    
        public int IdFilm { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public int Year { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int? Rate { get; set; }
        public string Link { get; set; }
        public bool Deleted { get; set; }
    
        public virtual ICollection<Genre> Genres { get; set; }
        public virtual ICollection<Country> Countries { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual Director Director { get; set; }
        public virtual ICollection<Actor> Actors { get; set; }
        public virtual Director Director1 { get; set; }
	}
}
