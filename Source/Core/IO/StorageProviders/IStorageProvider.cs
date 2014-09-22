namespace SqlFramework.IO.StorageProviders
{
    using System.IO;
    using CodeBuilders;

    public interface IStorageProvider
    {
        string Combine(params string[] paths);
        
        ICodeBuilder CreateOrOpenCodeWriter(string fileName, string indentation);
        
        Stream CreateOrOpenStream(string fileName);
        
        bool DirectoryExists(string directory);
        
        bool FileExists(string fileName);
        
        string GetDirectoryName(string path);
        
        Stream OpenStream(string fileName);
    }
}