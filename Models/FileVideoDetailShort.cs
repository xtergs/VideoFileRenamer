﻿using System;

namespace VideoFileRenamer.Download
{
	[Serializable]
	public class FileVideoDetailShort
	{
		private string name;
		private string originalName;
		private bool isAdd = true;

		public string Image { get; set; }

		public bool IsAdd
		{
			get { return isAdd; }
			set { isAdd = value; }
		}

		public string Name {
			get { return name; }
			set { name = value.Trim(); }
		} 

		public string OriginalName
		{
			get { return originalName; }
			set { originalName = value.Trim(); }
		}

		public int Year { get; set; }

		public string Link { get; set; }

		public override string ToString()
		{
			return Name + "[" + originalName + "] " + "(" + Year + ")";
		}
	}
}
