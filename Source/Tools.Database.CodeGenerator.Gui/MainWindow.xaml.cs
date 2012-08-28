using System.Windows;
using System.Windows.Controls;
using Tools.Database.CodeGenerator.Gui.ViewModels;
using System.Windows.Input;
using System.Windows.Data;
using Forms = System.Windows.Forms;



namespace Tools.Database.CodeGenerator.Gui
{

	public partial class MainWindow : Window
	{

		public MainWindow()
		{
			InitializeComponent();
			this.model = new MainViewModel();
			this.DataContext = this.model;
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
			var dialog = new Forms.FolderBrowserDialog();

			Forms.DialogResult result = dialog.ShowDialog();

			if (result == System.Windows.Forms.DialogResult.OK)
			{
				tbFolder.Text = dialog.SelectedPath;
			}
		}

		private void OnGenerateClick(object sender, RoutedEventArgs e)
		{
			bool valid = Validate(this.tbConfiguration);
			valid = Validate(this.tbFilename) && valid;
			valid = Validate(this.tbFolder) && valid;

			if (valid)
			{
				
			}
		}

		private bool Validate(TextBox tb)
		{
			BindingExpression expression = tb.GetBindingExpression(TextBox.TextProperty);
			expression.UpdateSource();
			return expression.HasError;
		}



		private readonly MainViewModel model;

	}
}
