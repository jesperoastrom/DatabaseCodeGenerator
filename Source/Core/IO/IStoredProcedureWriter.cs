using SqlFramework.Data.Models;

namespace SqlFramework.IO
{
    public interface IStoredProcedureWriter
    {
        void Write(SchemaCollection<StoredProcedureModel> storedProcedures);
    }
}