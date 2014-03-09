using SqlFramework.Data.Models;

namespace SqlFramework.Data.Builders
{
    public interface ISchemaElementCollectionBuilder
    {
        ISchemaElementCollection<TItem> Build<TItem>(string schemaName);
    }
}