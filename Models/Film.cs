using System.Windows.Controls.Primitives;

namespace VideoFileRenamer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Film
    {
        public Film()
        {
            Files = new HashSet<File>();
            Countries = new HashSet<Country>();
            Actors = new HashSet<Actor>();
            Genres = new HashSet<Genre>();
	        Deleted = false;
	        Added = DateTime.UtcNow;
        }

        public int FilmID { get; set; }

		public DateTime Added { get; set; }

        public string Name { get; set; }

        public string OriginalName { get; set; }

        public int Year { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public int? Rate { get; set; }

        public string Link { get; set; }

        public bool Deleted { get; set; }

        public int? Director_DirectorID { get; set; }

        public virtual Director Director { get; set; }

        public virtual ICollection<File> Files { get; set; }

        public virtual ICollection<Country> Countries { get; set; }

        public virtual ICollection<Actor> Actors { get; set; }

        public virtual ICollection<Genre> Genres { get; set; }
    }
}
