using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VideoFileRenamer.Download;

namespace VideoFileRenamer.UI
{
	/// <summary>
	/// Interaction logic for Tasks.xaml
	/// </summary>
	public partial class Tasks : Window
	{
		public Tasks()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			AppEngine.Create().ProgressStatus += Tasks_ProgressStatus;
			AppEngine.Create().ChangedStatus += Tasks_ChangedStatus;
			AppEngine.Create().FindFilmsStarted+=Tasks_FindFilmsStarted;
			AppEngine.Create().FindFilmsFinished += Tasks_FindFilmsFinished;
		}

		void Tasks_FindFilmsFinished()
		{
			Dispatcher.InvokeAsync(() =>
				ProgressGrid.Visibility = Visibility.Collapsed);
		}

		private void Tasks_FindFilmsStarted()
		{
			Dispatcher.InvokeAsync(() =>
				ProgressGrid.Visibility = Visibility.Visible);
		}

		void Tasks_ChangedStatus(string message)
		{
			Dispatcher.InvokeAsync(() => 
				MessagePanel.Items.Add(message));
		}

		void Tasks_ProgressStatus(int n, int count, string message)
		{
			Dispatcher.InvokeAsync(() =>
			{
				Message.Text = message;
				ProgressBar.Maximum = count;
				ProgressBar.Value = n;
			});
		}
	}
}
