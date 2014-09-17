namespace SqlFramework.Data.Models
{
    using System.Collections.Generic;

    public sealed class StoredProcedureResultModel
    {
        public List<ColumnModel> Columns { get; set; }
    }
}