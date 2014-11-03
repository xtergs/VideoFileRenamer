namespace VideoFileRenamer.Models
{
	using System.Collections.Generic;

	public partial class Country
    {
        public Country()
        {
            Films = new HashSet<Film>();
        }

        public int CountryID { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Film> Films { get; set; }
    }
}
