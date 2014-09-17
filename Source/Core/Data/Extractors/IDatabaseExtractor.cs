namespace SqlFramework.Data.Extractors
{
    using Models;

    public interface IDatabaseExtractor
    {
        DatabaseModel Extract(Configuration.DatabaseConfiguration configuration);
    }
}