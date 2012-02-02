using System.Collections.Generic;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	internal sealed class DatabaseModel
	{

		public IEnumerable<UserDefinedTableTypeModel> UserDefinedTableTypes { get; set; }
		public IEnumerable<StoredProcedureModel> StoredProcedures { get; set; }

	}

}
