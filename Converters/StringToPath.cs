﻿using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace VideoFileRenamer
{
	class StringToPath:IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Directory.GetCurrentDirectory() + "\\" + value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
