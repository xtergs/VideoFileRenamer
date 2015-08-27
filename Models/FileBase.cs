using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using MediaInfoDotNet;

namespace VideoFileRenamer.Models
{
	[Serializable]
	public class FileBase
	{
		public static string videoQualitiesPattern =
			@"webdlrip|DVDRip|HDTVrip|HDrip|BluRay|WEBRip|web-dlrip|TVRip|WEB-DL|BDRip|WEBRip|SATRip";
		public static string resolutionPattern = @"1080p|720p|1080|720";
		public static string removeCharsPattern = @"ELEKTRI4KA|HQCLUB|HQ-ViDEO|HDCLUB|novafilm\.tv|nnm-club\.ru|tfile\.ru|HELLYWOOD|scarabey\.org|([0-9]+MB)|[[\]\.():!?/\\\-,]";
		public static string audioQualitiesPattern = @"чистый звук|D.line|D.Line|DTS|.D|лицензия|Звук TS";
		
		protected FileBase()
		{
		}

		public FileBase(FileInfo info)
		{
			FileName = info.Name;
			Size = info.Length;
			Path = info.DirectoryName;
			Created = info.CreationTimeUtc;
			Modified = info.LastWriteTimeUtc;
			Quality = GetQuality(FileName.ToLowerInvariant());
			SearchName = GetSearchName(FileName);
			Part = "";
			QualityP = GetQualityP(info.Name);
			AutiodQuality = GetAudioQuality(info.Name.Substring(info.Name.Length/2));
		}

		public string AutiodQuality { get; set; }


		static string GetAudioQuality(string Name)
		{
			var matches = Regex.Matches(Name, audioQualitiesPattern);
			if (matches.Count > 0)
				return matches[0].Value;
			return "";
		}

		static string GetQualityP(string name)
		{
			var matches = Regex.Matches(name, resolutionPattern, RegexOptions.IgnoreCase);
			if (matches.Count > 0)
				return matches[0].Value;
			return "";
		}

		string Part { get; set; }

		[NotMapped]
		public MediaFile Media {
			get { return new MediaFile(FullPath); } }

		static string GetQuality(string fileName)
		{
			var matchs = Regex.Matches(fileName, videoQualitiesPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			if (matchs.Count >= 1)
				return matchs[0].Value;
			return "";
			//for (int i = 0; i < videoQualities.Length; i++)
			//	if (fileName.ToLowerInvariant().Contains(videoQualities[i]))
			//		return i;
		}

		public static string GetSearchName(string str)
		{
			str = Regex.Replace(str, videoQualitiesPattern, "", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			str = Regex.Replace(str, resolutionPattern, "", RegexOptions.IgnoreCase);
			str = Regex.Replace(str, audioQualitiesPattern, "");
			str = Regex.Replace(str, removeCharsPattern, " ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			StringBuilder builder = new StringBuilder(System.IO.Path.GetFileNameWithoutExtension(str.ToLowerInvariant().Trim()));
			builder.Replace("    ", " ").Replace("   ", " ").Replace("  ", " ");
			return builder.ToString().Trim();
		}

		public string SearchName { get; set; }

		public string QualityP { get; set; }
		public string MD5 { get; set; }

		public string FileName { get; set; }

		public long Size { get; set; }

		public string Path { get; set; }

		public bool Deleted { get; set; }

		public string Quality { get; set; }

		[NotMapped]
		public string FullPath
		{
			get { return Path + System.IO.Path.DirectorySeparatorChar + FileName; }
		}

		public DateTime Created { get; set; }

		public DateTime Modified { get; set; }

		public string SizeString
		{
			get
			{
				double size = Size;
				int prefi = 0;
				while (size >= 1024)
				{
					size /= 1024;
					prefi++;
				}
				string prefix = " B";
				switch (prefi)
				{
					case 0:
						prefix = " B";
						break;
					case 1:
						prefix = " KB";
						break;
					case 2:
						prefix = " MB";
						break;
					case 3:
						prefix = " GB";
						break;
					case 4: prefix = " TB";
						break;
					case 5:
						prefix = " PB";
						break;
				}
				return size.ToString("F2") + prefix;
			}
		}
	}
}
