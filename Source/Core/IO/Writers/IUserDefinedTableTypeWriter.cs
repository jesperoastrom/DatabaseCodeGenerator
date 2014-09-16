using SqlFramework.Data.Models;

namespace SqlFramework.IO.Writers
{
    public interface IUserDefinedTableTypeWriter
    {
        void Write(SchemaCollection<UserDefinedTableTypeModel> userDefinedTableTypes);
    }
}