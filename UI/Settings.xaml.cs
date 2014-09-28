using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FolderPickerLib;
using VideoFileRenamer.Annotations;
using VideoFileRenamer.Download;
using Path = System.IO.Path;

namespace VideoFileRenamer.UI
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : Window, INotifyPropertyChanged
	{
		public Settings()
		{
			InitializeComponent();
		}

		private StringCollection dirsList;
		private string pattern;

		public string Pattern
		{
			get { return pattern; }
			set
			{
				pattern = value;
				OnPropertyChanged();
			}
		}

		public StringCollection DirsList
		{
			get { return dirsList; }
			set
			{
				dirsList = value;
				OnPropertyChanged();
			}
		}

		private void Grid_Loaded(object sender, RoutedEventArgs e)
		{
			var engine = AppEngine.Create();

			DirsList = (StringCollection)Properties.Settings.Default[engine.Dirs];
			if (DirsList == null)
				DirsList = new StringCollection();
			Pattern = (string) Properties.Settings.Default[engine.Pattern];
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			AddDir();
		}

		void ValidatePath()
		{
			if (!Directory.Exists(pathNew.Text))
				Directory.CreateDirectory(pathNew.Text);
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			DeleteDir();
		}

		void DeleteDir()
		{
			if (ListOfDirs.SelectedIndex < 0)
				return;
			dirsList.Remove((string)ListOfDirs.SelectedItem);
			ListOfDirs.Items.Refresh();
			ListOfDirs.SelectedIndex = 0;
		}

		void AddDir()
		{
			ValidatePath();
			if (dirsList.IndexOf(pathNew.Text) >= 0)
				return;
			DirsList.Add(pathNew.Text);
			ListOfDirs.Items.Refresh();
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			SaveSettigns();
		}

		void SaveSettigns()
		{
			var engine = AppEngine.Create();
			//System.Collections.Specialized.StringCollection dd = new StringCollection();
			//dd.AddRange(dirsList.ToArray());
			Properties.Settings.Default[engine.Dirs] = DirsList;
			Properties.Settings.Default[engine.Pattern] = Pattern;
			Properties.Settings.Default.Save();
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			Close();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Button_Click_4(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			DialogResult result = fbd.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				pathNew.Text = fbd.SelectedPath;
			}
		}
	}
}
