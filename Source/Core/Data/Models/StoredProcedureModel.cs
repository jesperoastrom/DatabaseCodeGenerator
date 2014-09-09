using System.Collections.Generic;

namespace SqlFramework.Data.Models
{
    public sealed class StoredProcedureModel
    {
        public IDatabaseName DatabaseName { get; set; }
        public ITypeName TypeName { get; set; }
        public List<ParameterModel> Parameters { get; set; }
        public List<ParameterModel> OutputParameters { get; set; }
        public List<StoredProcedureResultModel> Results { get; set; }

        public override string ToString()
        {
            return DatabaseName == null ? null : DatabaseName.ToString();
        }
    }
}