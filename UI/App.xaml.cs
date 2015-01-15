using System.Windows;
using VideoFileRenamer.ViewModels;

namespace VideoFileRenamer.Download
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		#region Overrides of Application

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			MainWindow mainWindow = new MainWindow() {DataContext = new FilmsViewModel()};
			mainWindow.Show();
		}

		#endregion

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			AppEngine.Create().Backup();
		}
	}
}
