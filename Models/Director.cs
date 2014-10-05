namespace VideoFileRenamer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Director
    {
        public Director()
        {
            Films = new HashSet<Film>();
        }

        public int DirectorID { get; set; }

        public string FistName { get; set; }

        public string SecondName { get; set; }

        public string Link { get; set; }

        public virtual ICollection<Film> Films { get; set; }
    }
}
