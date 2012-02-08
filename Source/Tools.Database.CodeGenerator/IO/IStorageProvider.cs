using System.IO;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	public interface IStorageProvider
	{

		bool OutputDirectoryExists(string directory);
		bool ConfigurationFileExists(string fileName);
		ICodeWriter CreateOrOpenOutputWriter(string fileName, string indentation);
		Stream OpenConfigurationFile(string fileName);

	}

}
