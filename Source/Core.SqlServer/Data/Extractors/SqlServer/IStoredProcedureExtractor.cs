namespace SqlFramework.Data.Extractors.SqlServer
{
    using Configuration;
    using Microsoft.SqlServer.Management.Smo;
    using Models;

    public interface IStoredProcedureExtractor
    {
        SchemaCollection<StoredProcedureModel> Extract(DatabaseConfiguration configuration, StoredProcedureCollection storedProcedures);
    }
}