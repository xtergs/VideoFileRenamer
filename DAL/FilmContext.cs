using System.Data.Entity;
using VideoFileRenamer.Models;

namespace VideoFileRenamer.DAL
{
	public  class FilmContext : DbContext
	{
		public FilmContext(string connectinString)
			: base(connectinString)
		{
		}
		public virtual DbSet<Actor> Actors { get; set; }
		public virtual DbSet<Country> Countries { get; set; }
		public virtual DbSet<Director> Directors { get; set; }
		public virtual DbSet<File> Files { get; set; }
		public virtual DbSet<Film> Films { get; set; }
		public virtual DbSet<Genre> Genres { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Actor>()
				.Property(e => e.FistName)
				.IsUnicode(false);

			modelBuilder.Entity<Actor>()
				.Property(e => e.SecondName)
				.IsUnicode(false);

			modelBuilder.Entity<Actor>()
				.HasMany(e => e.Films)
				.WithMany(e => e.Actors)
				.Map(m => m.ToTable("FilmActor").MapLeftKey("Actors_IdActor"));

			modelBuilder.Entity<Country>()
				.HasMany(e => e.Films)
				.WithMany(e => e.Countries)
				.Map(m => m.ToTable("Film_Country").MapLeftKey("Countries_IdCountry"));

			modelBuilder.Entity<Director>()
				.Property(e => e.FistName)
				.IsUnicode(false);

			modelBuilder.Entity<Director>()
				.Property(e => e.SecondName)
				.IsUnicode(false);

			modelBuilder.Entity<Director>()
				.HasMany(e => e.Films)
				.WithOptional(e => e.Director)
				.HasForeignKey(e => e.Director_DirectorID);

			modelBuilder.Entity<Film>()
				.Property(e => e.Name)
				.IsUnicode(false);

			modelBuilder.Entity<Film>()
				.Property(e => e.OriginalName)
				.IsUnicode(false);

			modelBuilder.Entity<Film>()
				.HasMany(e => e.Files)
				.WithOptional(e => e.Film)
				.HasForeignKey(e => e.Film_FilmID);

			modelBuilder.Entity<Film>()
				.HasMany(e => e.Genres)
				.WithMany(e => e.Films)
				.Map(m => m.ToTable("Genre_Film").MapLeftKey("Films_IdFilm").MapRightKey("Genres_IdGenre"));

			modelBuilder.Entity<Genre>()
				.Property(e => e.Name)
				.IsUnicode(false);
		}
	}
}