using System.Data.Entity;
using System.IO;
using System.Linq;
using File = VideoFileRenamer.Models.File;

namespace VideoFileRenamer.DAL
{
	class FileRepository : GenericRepository<File>
	{


		public FileRepository(FilmContext context) : base(context)
		{
		}

		public IQueryable<File> GetAllData()
		{
			var d = dbSet.Include(x=>x.Film);
			return d;
		}

		public override bool IsContain(File file)
		{
			return dbSet.Any(x => x.FileName == file.FileName && x.Size == file.Size);
		}

		public bool IsContain(FileInfo file)
		{
			bool b = dbSet.Any(x => x.FileName == file.Name && x.Size == file.Length);
			return b;
		}

		public override void Delete(File entityToDelete)
		{
			if (entityToDelete.Film.Files.Count <= 1)
				entityToDelete.Film.Deleted = true;
			base.Delete(entityToDelete);
		}
	}
}
