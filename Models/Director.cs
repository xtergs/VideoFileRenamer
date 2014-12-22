namespace VideoFileRenamer.Models
{
	using System.Collections.Generic;

	public partial class Director :Person
    {
	    public override string ToString()
	    {
		    return FirstName + " " + SecondName;
	    }

	    public int DirectorID { get; set; }

        //public virtual ICollection<Film> Films { get; set; }
    }
}
