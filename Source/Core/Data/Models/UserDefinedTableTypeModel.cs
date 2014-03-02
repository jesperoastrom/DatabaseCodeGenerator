using System.Collections.Generic;



namespace SqlFramework.Data.Models
{

	public sealed class UserDefinedTableTypeModel
	{

		public DatabaseName DatabaseName { get; set; }
		public TypeName TypeName { get; set; }
		public List<ColumnModel> Columns { get; set; }



		public override string ToString()
		{
			return this.DatabaseName == null ? null : this.DatabaseName.ToString();
		}

	}

}
