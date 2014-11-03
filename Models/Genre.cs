namespace VideoFileRenamer.Models
{
	using System.Collections.Generic;

	public class Genre
    {
        public Genre()
        {
            Films = new HashSet<Film>();
        }

	    public override bool Equals(object obj)
	    {
		    if (obj.GetType() != typeof (Genre))
			    return false;
		    var temp = (Genre) obj;
		    if (temp.Name == Name)
			    return true;
			return false;
	    }

	    public bool Equals(Genre s2)
	    {
		    return true;
	    }

	    public override string ToString()
	    {
		    return Name;
	    }

	    public int GenreID { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Film> Films { get; set; }
    }
}
