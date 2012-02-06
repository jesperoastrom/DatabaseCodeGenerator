namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	internal sealed class ColumnModel
	{

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
			set
			{
				this.databaseName = value;
				this.ParameterName = value.ToParameterName();
				this.PropertyName = value.ToPropertyName();
			}
		}
		public string ParameterName { get; private set; }
		public string PropertyName { get; private set; }
		public string ClrType { get; set; }



		public override string ToString()
		{
			return this.DatabaseName;
		}



		private string databaseName;

	}

}
