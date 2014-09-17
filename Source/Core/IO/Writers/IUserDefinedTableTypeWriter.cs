namespace SqlFramework.IO.Writers
{
    using Data.Models;

    public interface IUserDefinedTableTypeWriter
    {
        void Write(SchemaCollection<UserDefinedTableTypeModel> userDefinedTableTypes);
    }
}