namespace SqlFramework.Gui.Configuration
{
    using System.ComponentModel;

    public sealed class GuiConfiguration : INotifyPropertyChanged
    {
        public string ConnectionString
        {
            get { return _connectionString; }
            set { UpdatePropertyValue("ConnectionString", ref _connectionString, ref value); }
        }

        public string OutputFolder
        {
            get { return _outputFolder; }
            set { UpdatePropertyValue("Folder", ref _outputFolder, ref value); }
        }

        public string OutputFilename
        {
            get { return _outputFilename; }
            set { UpdatePropertyValue("Filename", ref _outputFilename, ref value); }
        }

        public string ConfigurationFilePath
        {
            get { return _configurationFilePath; }
            set { UpdatePropertyValue("ConfigurationFilePath", ref _configurationFilePath, ref value); }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool UpdatePropertyValue<T>(string propertyName, ref T oldValue, ref T newValue)
        {
            if (oldValue == null && newValue == null)
            {
                return false;
            }

            if ((oldValue == null && newValue != null) || !oldValue.Equals((T)newValue))
            {
                oldValue = newValue;
                RaisePropertyChanged(propertyName);
                return true;
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private string _configurationFilePath;
        
        private string _connectionString;
        
        private string _outputFilename;
        
        private string _outputFolder;
    }
}