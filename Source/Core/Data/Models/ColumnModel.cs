namespace SqlFramework.Data.Models
{

	public sealed class ColumnModel
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
		public ClrType ClrType { get; set; }



		public override string ToString()
		{
			return this.DatabaseName;
		}



		private string databaseName;

	}

}
