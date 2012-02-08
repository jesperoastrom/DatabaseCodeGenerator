using System.Data;
using Smo = Microsoft.SqlServer.Management.Smo;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	public sealed class ParameterModel
	{

		public bool IsOutput { get; set; }
		public Smo.SqlDataType SqlDataType { get; set; }
		public SqlDbType SqlDbType { get; set; }
		public ColumnModel Column { get; set; }
		/// <summary>
		/// Indicates if column should be included in formatting query.
		/// </summary>
		public bool IncludeInFmtOnlyQuery()
		{
			//TODO More here
			return this.SqlDataType != Smo.SqlDataType.UserDefinedTableType;
		}

	}

}
