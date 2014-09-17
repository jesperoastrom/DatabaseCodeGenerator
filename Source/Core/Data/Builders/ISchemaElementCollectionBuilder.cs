namespace SqlFramework.Data.Builders
{
    using Models;

    public interface ISchemaElementCollectionBuilder
    {
        ISchemaElementCollection<TItem> Build<TItem>(string schemaName);
    }
}