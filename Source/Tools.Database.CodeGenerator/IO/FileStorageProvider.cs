using System.IO;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	public sealed class FileStorageProvider : IStorageProvider
	{

		public bool OutputDirectoryExists(string directory)
		{
			return Directory.Exists(directory);
		}

		public bool ConfigurationFileExists(string fileName)
		{
			return File.Exists(fileName);
		}

		public ICodeWriter CreateOrOpenOutputWriter(string fileName, string indentation)
		{
			return new StreamCodeWriter(File.Open(fileName, FileMode.Create), indentation);
		}

		public Stream OpenConfigurationFile(string fileName)
		{
			return new FileStream(fileName, FileMode.Open);
		}

	}

}
