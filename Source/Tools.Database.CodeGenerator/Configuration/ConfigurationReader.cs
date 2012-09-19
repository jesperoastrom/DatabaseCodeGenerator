using System;
using System.IO;
using Flip.Tools.Database.CodeGenerator.IO;



namespace Flip.Tools.Database.CodeGenerator.Configuration
{

	public sealed class ConfigurationReader : IConfigurationReader
	{

		public ConfigurationReader(ITextWriter errorOutput, IStorageProvider storageProvider)
		{
			this.errorOutput = errorOutput;
			this.storageProvider = storageProvider;
		}



		public bool TryRead(string file, out DatabaseConfiguration configuration)
		{
			if (!this.storageProvider.FileExists(file))
			{
				this.errorOutput.WriteLine("Unable to find file '" + file + "'");
				configuration = null;
				return false;
			}

			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(DatabaseConfiguration));

			try
			{
				using (var stream = this.storageProvider.OpenStream(file))
				{
					configuration = (DatabaseConfiguration)serializer.Deserialize(stream);
					configuration.TableTypeNamespaceFromStoredProcedure =
						configuration.StoredProcedures.Namespace.GetShortestNamespace(configuration.UserDefinedTableTypes.Namespace);
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



		private readonly IStorageProvider storageProvider;
		private readonly ITextWriter errorOutput;

	}

}
