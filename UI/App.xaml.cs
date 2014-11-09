﻿using System.Windows;

namespace VideoFileRenamer.Download
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Exit(object sender, ExitEventArgs e)
		{
			AppEngine.Create().Backup();
		}
	}
}
