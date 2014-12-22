namespace VideoFileRenamer.Models
{
	using System.Collections.Generic;

	public class Actor:Person
    {
        public Actor()
        {
            Films = new HashSet<Film>();
        }

        public int ActorID { get; set; }

        
    }
}
