using System.Data;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	internal sealed class ColumnModel
	{

		public string DatabaseName { get; set; }
		public SqlDbType SqlDbType { get; set; }
		public string ParameterName { get; set; }
		public string PropertyName { get; set; }
		public string ClrType { get; set; }



		public override string ToString()
		{
			return this.DatabaseName;
		}

	}

}
