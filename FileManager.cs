using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaInfoDotNet;
using VideoFileRenamer.DAL;
using VideoFileRenamer.Models;
using File = VideoFileRenamer.Models.File;

namespace VideoFileRenamer
{
	public class FileManager
	{
		private FilmContext context;
		public FileManager(FilmContext context)
		{
			this.context = context;
		}

		
	}
}
