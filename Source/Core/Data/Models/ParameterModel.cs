using System.Data;
using Smo = Microsoft.SqlServer.Management.Smo;



namespace SqlFramework.Data.Models
{

	public sealed class ParameterModel
	{
		public int? Size { get; set; }
		public int? Scale { get; set; }
		public int? Precision { get; set; }
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
