using System.Collections.Generic;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	internal sealed class StoredProcedureModel
	{

		public DatabaseName DatabaseName { get; set; }
		public TypeName TypeName { get; set; }
		public List<ParameterModel> Parameters { get; set; }
		public List<ResultModel> Results { get; set; }



		public override string ToString()
		{
			return this.DatabaseName == null ? null : this.DatabaseName.ToString();
		}

	}

}
