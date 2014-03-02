using System.ComponentModel;



namespace Tools.Database.CodeGenerator.Gui.Configuration
{

	public sealed class GuiConfiguration : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler PropertyChanged;


		private string _connectionString;
		public string ConnectionString
		{
			get { return _connectionString; }
			set { this.UpdatePropertyValue("ConnectionString", ref _connectionString, ref value); }
		}

		private string _outputFolder;
		public string OutputFolder
		{
			get { return _outputFolder; }
			set { this.UpdatePropertyValue("Folder", ref _outputFolder, ref value); }
		}

		private string _outputFilename;
		public string OutputFilename
		{
			get { return _outputFilename; }
			set { this.UpdatePropertyValue("Filename", ref _outputFilename, ref value); }
		}

		private string _configurationFilePath;
		public string ConfigurationFilePath
		{
			get { return _configurationFilePath; }
			set { this.UpdatePropertyValue("ConfigurationFilePath", ref _configurationFilePath, ref value); }
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

		private void RaisePropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

	}

}
