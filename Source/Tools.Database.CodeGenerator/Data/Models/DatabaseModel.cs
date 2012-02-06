using System.Collections.Generic;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	internal sealed class DatabaseModel
	{

		public SchemaCollection<UserDefinedTableTypeModel> UserDefinedTableTypes { get; set; }
		public SchemaCollection<StoredProcedureModel> StoredProcedures { get; set; }

	}

}
