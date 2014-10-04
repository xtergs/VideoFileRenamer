using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.DAL
{
	internal class FilmContext: DbContext
	{
		public FilmContext(string connectionString)
			: base(connectionString)
		{
			
		}

		public DbSet<Film> Films { get; set; }
		public DbSet<File> Files { get; set; }
		public DbSet<Director> Directors { get; set; }
		public DbSet<Genre> Genres { get; set; }
		public DbSet<Country> Countries { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			
		}
	}
}