using SqlFramework.Data.Models;

namespace SqlFramework.Data
{
    public interface IDatabaseExtractor
    {
        DatabaseModel Extract(Configuration.DatabaseConfiguration configuration);
    }
}