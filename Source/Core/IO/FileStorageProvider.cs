using System.IO;



namespace SqlFramework.IO
{

	public sealed class FileStorageProvider : IStorageProvider
	{

		public string Combine(params string[] paths)
		{
			return Path.Combine(paths);
		}

		public string GetDirectoryName(string path)
		{
			return Path.GetDirectoryName(path);
		}

		public bool DirectoryExists(string directory)
		{
			return Directory.Exists(directory);
		}

		public bool FileExists(string fileName)
		{
			return File.Exists(fileName);
		}

		public ICodeWriter CreateOrOpenCodeWriter(string fileName, string indentation)
		{
			return new StreamCodeWriter(File.Open(fileName, FileMode.Create), indentation);
		}

		public Stream OpenStream(string fileName)
		{
			return new FileStream(fileName, FileMode.Open);
		}

		public Stream CreateOrOpenStream(string fileName)
		{
			return new FileStream(fileName, FileMode.Create);
		}

	}

}
