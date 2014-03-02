using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autofac;
using SqlFramework.Configuration;
using SqlFramework.Data;
using SqlFramework.IO;
using Tools.Database.CodeGenerator.Gui.Configuration;
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

			this.guiConfigurationProvider = new GuiConfigurationProvider(new FileStorageProvider());
			this.guiConfiguration = this.guiConfigurationProvider.LoadConfiguration();
			this.guiConfiguration.PropertyChanged += ConfigurationPropertyChanged;
			this.Loaded += OnLoaded;
		}



		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			UpdateControlsWithValuesFromConfiguration();
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

				string outputFilePath = Path.Combine(tbFolder.Text, tbFilename.Text);

				try
				{
					if (!writer.WriteOutput(tbConfiguration.Text, outputFilePath, "\t"))
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
				UpdateConfigurationWithValuesFromControls();
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

		private void UpdateControlsWithValuesFromConfiguration()
		{
			tbConfiguration.Text = this.guiConfiguration.ConfigurationFilePath;
			tbConnectionString.Text = this.guiConfiguration.ConnectionString;
			tbFilename.Text = this.guiConfiguration.OutputFilename;
			tbFolder.Text = this.guiConfiguration.OutputFolder;
		}

		private void UpdateConfigurationWithValuesFromControls()
		{
			this.guiConfiguration.ConfigurationFilePath = tbConfiguration.Text;
			this.guiConfiguration.ConnectionString = tbConnectionString.Text;
			this.guiConfiguration.OutputFilename = tbFilename.Text;
			this.guiConfiguration.OutputFolder = tbFolder.Text;

			if (this.guiConfigurationHasChanged)
			{
				this.guiConfigurationProvider.SaveConfiguration(this.guiConfiguration);
			}
		}

		private void ConfigurationPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			this.guiConfigurationHasChanged = true;
		}



		private bool guiConfigurationHasChanged;
		private readonly GuiConfiguration guiConfiguration;
		private readonly GuiConfigurationProvider guiConfigurationProvider;
		private readonly MainViewModel model;

	}
}
