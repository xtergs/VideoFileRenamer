namespace VideoFileRenamer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Genre
    {
        public Genre()
        {
            Films = new HashSet<Film>();
        }

        public int GenreID { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Film> Films { get; set; }
    }
}
