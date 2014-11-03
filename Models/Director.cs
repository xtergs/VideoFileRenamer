namespace VideoFileRenamer.Models
{
	using System.Collections.Generic;

	public partial class Director
    {
        public Director()
        {
            Films = new HashSet<Film>();
        }

	    public override string ToString()
	    {
		    return FistName + " " + SecondName;
	    }

	    public int DirectorID { get; set; }

        public string FistName { get; set; }

        public string SecondName { get; set; }

        public string Link { get; set; }

        public virtual ICollection<Film> Films { get; set; }
    }
}
