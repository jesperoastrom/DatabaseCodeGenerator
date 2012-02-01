using System;
using System.IO;



namespace Flip.Tools.Database.CodeGenerator.Configuration
{

	public sealed class ConfigurationReader
	{

		public ConfigurationReader(string file, System.IO.TextWriter errorOutput)
		{
			this.file = file;
			this.errorOutput = errorOutput;
		}



		public bool TryRead(out DatabaseConfiguration configuration)
		{
			if (!File.Exists(this.file))
			{
				this.errorOutput.WriteLine("Unable to find file '" + this.file + "'");
				configuration = null;
				return false;
			}

			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(DatabaseConfiguration));

			try
			{
				using (var stream = new FileStream(this.file, FileMode.Open))
				{
					configuration = (DatabaseConfiguration)serializer.Deserialize(stream);
					return true;
				}
			}
			catch (Exception ex)
			{
				WriteException(ex);
				configuration = null;
				return false;
			}
		}



		private void WriteException(Exception ex)
		{
			this.errorOutput.WriteLine(ex.Message);
			this.errorOutput.WriteLine(ex.StackTrace);
			if (ex.InnerException != null)
			{
				WriteException(ex.InnerException);
			}
		}



		private readonly string file;
		private readonly System.IO.TextWriter errorOutput;

	}

}
