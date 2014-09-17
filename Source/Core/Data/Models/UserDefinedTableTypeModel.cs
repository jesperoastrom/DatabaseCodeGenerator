namespace SqlFramework.Data.Models
{
    using System.Collections.Generic;

    public sealed class UserDefinedTableTypeModel
    {
        public IDatabaseName DatabaseName { get; set; }

        public ITypeName TypeName { get; set; }

        public List<ColumnModel> Columns { get; set; }

        public override string ToString()
        {
            return DatabaseName == null ? null : DatabaseName.ToString();
        }
    }
}