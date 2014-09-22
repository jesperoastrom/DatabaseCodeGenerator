namespace SqlFramework.Gui.ViewModels
{
    using System;
    using System.ComponentModel;

    public sealed class MainViewModel : INotifyPropertyChanged
    {
        public string Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                if (_configuration != value)
                {
                    _configuration = value;
                    RaisePropertyChanged("Configuration");
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ApplicationException("Configuration may not be empty");
                    }
                }
            }
        }

        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                if (_filename != value)
                {
                    _filename = value;
                    RaisePropertyChanged("Filename");
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ApplicationException("Filename may not be empty");
                    }
                }
            }
        }

        public string Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                if (_folder != value)
                {
                    _folder = value;
                    RaisePropertyChanged("Folder");
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ApplicationException("Folder may not be empty");
                    }
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _configuration;
        private string _filename;
        private string _folder;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}