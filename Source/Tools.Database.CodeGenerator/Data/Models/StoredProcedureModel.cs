using System.Collections.Generic;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	internal sealed class StoredProcedureModel
	{

		public DatabaseName Name { get; set; }
		public IEnumerable<ParameterModel> Parameters { get; set; }



		public override string ToString()
		{
			return this.Name == null ? null : this.Name.ToString();
		}

	}

}
