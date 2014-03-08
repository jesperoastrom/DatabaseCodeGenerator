namespace SqlFramework.Configuration
{
    public interface IConfigurationReader
    {
        DatabaseConfiguration Read(string file);
    }
}