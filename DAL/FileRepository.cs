using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = VideoFileRenamer.Models.File;

namespace VideoFileRenamer.DAL
{
	class FileRepository : GenericRepository<File>
	{
		public FileRepository(FilmContext context) : base(context)
		{
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

	}
}
