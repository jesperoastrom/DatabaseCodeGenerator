namespace SqlFramework.Data.Builders
{
    using Models;

    public interface IColumnModelBuilder
    {
        IColumnModel Build(string databaseName, ClrType clrType);
    }
}