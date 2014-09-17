namespace SqlFramework.Data.Builders
{
    using Models;

    public sealed class DatabaseNameBuilder : IDatabaseNameBuilder
    {
        public DatabaseNameBuilder(IDatabaseToCodeNameConverter nameConverter)
        {
            _nameConverter = nameConverter;
        }

        public IDatabaseName Build(string schema, string name)
        {
            return new DatabaseName
                       {
                           SchemaName = schema,
                           Name = name,
                           EscapedName = _nameConverter.EscapeDatabaseName(name),
                           EscapedSchemaName = _nameConverter.EscapeDatabaseName(schema),
                           EscapedFullName = _nameConverter.EscapeDatabaseName(schema, name)
                       };
        }

        private readonly IDatabaseToCodeNameConverter _nameConverter;
    }
}