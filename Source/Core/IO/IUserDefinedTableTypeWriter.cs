using SqlFramework.Data.Models;

namespace SqlFramework.IO
{
    public interface IUserDefinedTableTypeWriter
    {
        void Write(SchemaCollection<UserDefinedTableTypeModel> userDefinedTableTypes);
    }
}