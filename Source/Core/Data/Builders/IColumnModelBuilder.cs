using SqlFramework.Data.Models;

namespace SqlFramework.Data.Builders
{
    public interface IColumnModelBuilder
    {
        IColumnModel Build(string databaseName, ClrType clrType);
    }
}