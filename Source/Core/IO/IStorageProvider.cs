using System.IO;



namespace SqlFramework.IO
{

	public interface IStorageProvider
	{

		string Combine(params string[] paths);
		string GetDirectoryName(string path);
		bool DirectoryExists(string directory);
		bool FileExists(string fileName);
		ICodeWriter CreateOrOpenCodeWriter(string fileName, string indentation);
		Stream OpenStream(string fileName);
		Stream CreateOrOpenStream(string fileName);

	}

}
