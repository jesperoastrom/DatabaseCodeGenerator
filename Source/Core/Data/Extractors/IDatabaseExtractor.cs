using SqlFramework.Data.Models;

namespace SqlFramework.Data.Extractors
{
    public interface IDatabaseExtractor
    {
        DatabaseModel Extract(Configuration.DatabaseConfiguration configuration);
    }
}