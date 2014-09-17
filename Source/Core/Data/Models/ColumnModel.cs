namespace SqlFramework.Data.Models
{
    public sealed class ColumnModel : IColumnModel
    {
        public override string ToString()
        {
            return DatabaseName;
        }

        public string DatabaseName { get; set; }

        public string ParameterName { get; set; }

        public string PropertyName { get; set; }

        public ClrType ClrType { get; set; }
    }
}