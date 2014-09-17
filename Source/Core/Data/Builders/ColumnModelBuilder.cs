namespace SqlFramework.Data.Builders
{
    using Models;

    public sealed class ColumnModelBuilder : IColumnModelBuilder
    {
        public ColumnModelBuilder(IDatabaseToCodeNameConverter nameConverter)
        {
            _nameConverter = nameConverter;
        }

        public IColumnModel Build(string databaseName, ClrType clrType)
        {
            return new ColumnModel
                       {
                           DatabaseName = databaseName,
                           ParameterName = _nameConverter.ToParameterName(databaseName),
                           PropertyName = _nameConverter.ToPropertyName(databaseName),
                           ClrType = clrType
                       };
        }

        private readonly IDatabaseToCodeNameConverter _nameConverter;
    }
}