using System.Windows;
using System.Windows.Forms;



namespace Tools.Database.CodeGenerator.Gui
{

	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void OnFileClick(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.OpenFileDialog();

			dialog.DefaultExt = ".xml";
			dialog.Filter = "XML File (.xml)|*.xml";

			bool? result = dialog.ShowDialog();

			if (result == true)
			{
				tbConfiguration.Text = dialog.FileName;
			}
		}

		private void OnFolderClick(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog();

			DialogResult result = dialog.ShowDialog();

			if (result == System.Windows.Forms.DialogResult.OK)
			{
				tbFolder.Text = dialog.SelectedPath;
			}
		}

	}
}
