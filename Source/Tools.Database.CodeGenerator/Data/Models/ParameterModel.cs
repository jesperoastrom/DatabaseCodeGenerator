using Smo = Microsoft.SqlServer.Management.Smo;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	public sealed class ParameterModel
	{

		public bool IsOutput { get; set; }
		public Smo.SqlDataType SqlDbType { get; set; }
		public ColumnModel Column { get; set; }
		public bool IncludeInFmtOnlyQuery()
		{
			//TODO More here
			return this.SqlDbType != Smo.SqlDataType.UserDefinedTableType;
		}

	}

}
