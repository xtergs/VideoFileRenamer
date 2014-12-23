using System.Data.Entity;
using System.IO;
using System.Linq;
using File = VideoFileRenamer.Models.File;

namespace VideoFileRenamer.DAL
{
	public class FileRepository : GenericRepository<File>
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

		public File Contain(File file)
		{
			return dbSet.AsParallel().SingleOrDefault(x =>x.FileName == file.FileName && x.Size == file.Size);
		}

		#region Overrides of GenericRepository<File>

		public override File Add(File entity)
		{
			var ent = base.Add(entity);
			ent.Deleted = false;
			return ent;
		}

		#endregion

		public bool IsContain(FileInfo file)
		{
			bool b = dbSet.Any(x => x.FileName == file.Name && x.Size == file.Length);
			return b;
		}

		public override void Delete(File entityToDelete)
		{
			entityToDelete.Deleted = true;
			if (entityToDelete.Film.Files.Count(x=>x.Deleted == false) <= 0)
				entityToDelete.Film.Deleted = true;
			//base.Delete(entityToDelete);
		}
	}
}
