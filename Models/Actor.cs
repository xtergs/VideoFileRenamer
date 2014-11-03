namespace VideoFileRenamer.Models
{
	using System.Collections.Generic;

	public class Actor
    {
        public Actor()
        {
            Films = new HashSet<Film>();
        }

        public int ActorID { get; set; }

        public string FistName { get; set; }

        public string SecondName { get; set; }

        public virtual ICollection<Film> Films { get; set; }
    }
}
