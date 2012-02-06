using System.Data;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	internal sealed class ParameterModel
	{

		public bool IsOutput { get; set; }
		public SqlDbType SqlDbType { get; set; }
		public ColumnModel Column { get; set; }

	}

}
