using System.ComponentModel;
using System;



namespace Tools.Database.CodeGenerator.Gui.ViewModels
{

	public sealed class MainViewModel : INotifyPropertyChanged
	{

		private string configuration;
		public string Configuration
		{
			get { return this.configuration; }
			set
			{
				if (this.configuration != value)
				{
					this.configuration = value;
					RaisePropertyChanged("Configuration");
					if (string.IsNullOrWhiteSpace(value))
					{
						throw new ApplicationException("Configuration may not be empty");
					}
				}
			}
		}

		private string folder;
		public string Folder
		{
			get { return this.folder; }
			set
			{
				if (this.folder != value)
				{
					this.folder = value;
					RaisePropertyChanged("Folder");
					if (string.IsNullOrWhiteSpace(value))
					{
						throw new ApplicationException("Folder may not be empty");
					}
				}
			}
		}

		private string filename;
		public string Filename
		{
			get { return this.filename; }
			set
			{
				if (this.filename != value)
				{
					this.filename = value;
					RaisePropertyChanged("Filename");
					if (string.IsNullOrWhiteSpace(value))
					{
						throw new ApplicationException("Filename may not be empty");
					}
				}
			}
		}



		public event PropertyChangedEventHandler PropertyChanged;



		private void RaisePropertyChanged(string propertyName)
		{
			var handler = this.PropertyChanged;

			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

	}

}
