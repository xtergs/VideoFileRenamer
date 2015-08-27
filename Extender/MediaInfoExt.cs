using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaInfoDotNet;
using MediaInfoLib;

namespace VideoFileRenamer.Extender
{
	public static class MediaFileExt
	{
		public static bool Equal(this MediaFile l, MediaFile r)
		{
			if (l.size == r.size && l.duration == r.duration && l.bitRate == r.bitRate && l.format == r.format)
				return true;
			return false;
		}
	}
}
