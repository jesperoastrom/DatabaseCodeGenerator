using System;



namespace Flip.Tools.Database.CodeGenerator.Data
{

	internal sealed class DatabaseName
	{

		public DatabaseName(string schemaName, string name)
		{
			this.SchemaName = schemaName;
			this.Name = name;
		}



		public string SchemaName
		{
			get
			{
				return this.schemaName;
			}
			set
			{
				this.schemaName = value;
				this.EscapedSchemaName = value.EscapeDatabaseName();
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				this.EscapedName = value.EscapeDatabaseName();
			}
		}

		public string EscapedSchemaName { get; private set; }

		public string EscapedName { get; private set; }

		public string EscapedFullName
		{
			get
			{
				return EscapedSchemaName + "." + EscapedName;
			}
		}



		public override string ToString()
		{
			return this.EscapedFullName;
		}



		private string schemaName;
		private string name;

	}

}
