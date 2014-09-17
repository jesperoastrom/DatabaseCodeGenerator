namespace SqlFramework.Data.Builders
{
    using Models;

    public sealed class SchemaElementCollectionBuilder : ISchemaElementCollectionBuilder
    {
        public SchemaElementCollectionBuilder(IDatabaseToCodeNameConverter nameConverter)
        {
            _nameConverter = nameConverter;
        }

        public ISchemaElementCollection<TItem> Build<TItem>(string schemaName)
        {
            return new SchemaElementCollection<TItem>
                       {
                           SchemaName = schemaName,
                           StaticTypeName = _nameConverter.ToTypeName(schemaName)
                       };
        }

        private readonly IDatabaseToCodeNameConverter _nameConverter;
    }
}