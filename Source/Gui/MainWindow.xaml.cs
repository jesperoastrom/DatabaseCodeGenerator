namespace SqlFramework
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Autofac;
    using Data;
    using DependencyInjection;
    using Gui.Configuration;
    using Gui.IO;
    using Gui.ViewModels;
    using IO.OutputDestinations;
    using IO.StorageProviders;
    using IO.Writers;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _model = new MainViewModel();
            DataContext = _model;

            _guiConfigurationProvider = new GuiConfigurationProvider(new FileStorageProvider());
            _guiConfiguration = _guiConfigurationProvider.LoadConfiguration();
            _guiConfiguration.PropertyChanged += ConfigurationPropertyChanged;
        }

        private IContainer CreateContainer(string connectionString)
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new ConnectionStringProvider(connectionString)).As<IConnectionStringProvider>().SingleInstance();
            builder.RegisterModule<CoreModule>();
            builder.RegisterInstance<IOutputDestination>(new TextBoxOutputDestination(tbMessages));
            return builder.Build();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateControlsWithValuesFromConfiguration();
        }

        private void UpdateConfigurationWithValuesFromControls()
        {
            _guiConfiguration.ConfigurationFilePath = tbConfiguration.Text;
            _guiConfiguration.ConnectionString = tbConnectionString.Text;
            _guiConfiguration.OutputFilename = tbFilename.Text;
            _guiConfiguration.OutputFolder = tbFolder.Text;

            if (_guiConfigurationHasChanged)
            {
                _guiConfigurationProvider.SaveConfiguration(_guiConfiguration);
            }
        }

        private void UpdateControlsWithValuesFromConfiguration()
        {
            tbConfiguration.Text = _guiConfiguration.ConfigurationFilePath;
            tbConnectionString.Text = _guiConfiguration.ConnectionString;
            tbFilename.Text = _guiConfiguration.OutputFilename;
            tbFolder.Text = _guiConfiguration.OutputFolder;
        }

        private bool Validate(TextBox tb)
        {
            BindingExpression expression = tb.GetBindingExpression(TextBox.TextProperty);
            expression.UpdateSource();
            return !expression.HasError;
        }

        private readonly GuiConfiguration _guiConfiguration;
        private readonly GuiConfigurationProvider _guiConfigurationProvider;
        private readonly MainViewModel _model;
        private bool _guiConfigurationHasChanged;

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
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                tbFolder.Text = dialog.SelectedPath;
            }
        }

        private void OnGenerateClick(object sender, RoutedEventArgs e)
        {
            bool valid = Validate(tbConfiguration);
            valid = Validate(tbFilename) && valid;
            valid = Validate(tbFolder) && valid;
            valid = Validate(tbConnectionString) && valid;

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

        private void ConfigurationPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _guiConfigurationHasChanged = true;
        }
    }
}