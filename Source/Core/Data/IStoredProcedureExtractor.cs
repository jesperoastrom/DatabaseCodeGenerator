using Microsoft.SqlServer.Management.Smo;
using SqlFramework.Configuration;
using SqlFramework.Data.Models;

namespace SqlFramework.Data
{
    public interface IStoredProcedureExtractor
    {
        SchemaCollection<StoredProcedureModel> Extract(DatabaseConfiguration configuration, StoredProcedureCollection storedProcedures);
    }
}