using System.Collections.Generic;



namespace SqlFramework.Data.Models
{

	public sealed class DatabaseModel
	{

		public SchemaCollection<UserDefinedTableTypeModel> UserDefinedTableTypes { get; set; }
		public SchemaCollection<StoredProcedureModel> StoredProcedures { get; set; }

	}

}
