namespace SqlFramework.IO.Writers
{
    using Data.Models;

    public interface IStoredProcedureWriter
    {
        void Write(SchemaCollection<StoredProcedureModel> storedProcedures);
    }
}