using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autofac;
using Flip.Tools.Database.CodeGenerator.Configuration;
using Flip.Tools.Database.CodeGenerator.Data;
using Flip.Tools.Database.CodeGenerator.IO;
using Tools.Database.CodeGenerator.Gui.IO;
using Tools.Database.CodeGenerator.Gui.ViewModels;
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
			valid = Validate(this.tbConnectionString) && valid;

			if (valid)
			{
				IContainer container = CreateContainer(tbConnectionString.Text);
				var writer = container.Resolve<IDatabaseWriter>();

				string filePath = Path.Combine(tbFolder.Text, tbFilename.Text);

				try
				{
					if (!writer.WriteOutput(tbConfiguration.Text, filePath, "\t"))
					{
						MessageBox.Show("Output could not be created.");
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(), "Output could not be created. An exception was thrown.");
					return;
				}
				MessageBox.Show("The output was created!");
			}
		}

		private bool Validate(TextBox tb)
		{
			BindingExpression expression = tb.GetBindingExpression(TextBox.TextProperty);
			expression.UpdateSource();
			return !expression.HasError;
		}

		private IContainer CreateContainer(string connectionString)
		{
			var builder = new ContainerBuilder();

			builder.Register(c => new ConnectionStringProvider(connectionString)).As<IConnectionStringProvider>().SingleInstance();
			builder.RegisterCodeGeneratorTypes();
			builder.RegisterInstance<ITextWriter>(new TextBoxTextWriter(this.tbMessages));
			return builder.Build();
		}



		private readonly MainViewModel model;

	}
}
