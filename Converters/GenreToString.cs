using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VideoFileRenamer.Models;

namespace VideoFileRenamer
{
	class GenreToString : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var result = new StringBuilder();
			foreach (var str in ((HashSet<Genre>)value))
			{
				result.Append(str.Name).Append(", ");
			}
			return result.Remove(result.Length - 1, 1).ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
