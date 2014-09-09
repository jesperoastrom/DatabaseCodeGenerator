using System.IO;

namespace SqlFramework.IO
{
    public interface IStorageProvider
    {
        string Combine(params string[] paths);
        ICodeWriter CreateOrOpenCodeWriter(string fileName, string indentation);
        Stream CreateOrOpenStream(string fileName);
        bool DirectoryExists(string directory);
        bool FileExists(string fileName);
        string GetDirectoryName(string path);
        Stream OpenStream(string fileName);
    }
}