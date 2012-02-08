using System.Collections.Generic;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	public sealed class UserDefinedTableTypeModel
	{

		public DatabaseName Name { get; set; }
		public IEnumerable<ColumnModel> Columns { get; set; }



		public override string ToString()
		{
			return this.Name == null ? null : this.Name.ToString();
		}

	}

}
