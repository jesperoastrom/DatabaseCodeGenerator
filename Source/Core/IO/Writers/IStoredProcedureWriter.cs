using SqlFramework.Data.Models;

namespace SqlFramework.IO.Writers
{
    public interface IStoredProcedureWriter
    {
        void Write(SchemaCollection<StoredProcedureModel> storedProcedures);
    }
}