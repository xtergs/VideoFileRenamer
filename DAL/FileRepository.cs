using System.Data.Entity;
using System.IO;
using System.Linq;
using VideoFileRenamer.Models;
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
			if (Contain(file) != null)
				return true;
			return false;
			
		}

		public FileBase Contain(FileBase file)
		{
			return dbSet.AsParallel().SingleOrDefault(x =>x.SearchName == file.SearchName && x.Size == file.Size);
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
			return IsContain(new File(file));
		}

		public File HaveSimilarName(File file)
		{

			return dbSet.AsParallel().FirstOrDefault(x => x.SearchName == file.SearchName ||
														   x.PrevSerarchName == file.SearchName);
		}

		public File HaveSimilarName(FileInfo file)
		{
			return HaveSimilarName(new File(file));
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
