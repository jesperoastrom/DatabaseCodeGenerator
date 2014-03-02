using System;



namespace SqlFramework.Data.Models
{

	public sealed class DatabaseName : IComparable<DatabaseName>, IComparable
	{

		public DatabaseName(string schemaName, string name)
		{
			this.SchemaName = schemaName;
			this.Name = name;
			this.EscapedSchemaName = schemaName.EscapeDatabaseName();
			this.EscapedName = name.EscapeDatabaseName();
			this.EscapedFullName = EscapedSchemaName + "." + EscapedName;
		}



		public string SchemaName { get; private set; }

		public string Name { get; private set; }

		public string EscapedSchemaName { get; private set; }

		public string EscapedName { get; private set; }

		public string EscapedFullName { get; private set; }



		public override string ToString()
		{
			return this.EscapedFullName;
		}



		int IComparable<DatabaseName>.CompareTo(DatabaseName other)
		{
			if (other == null)
			{
				return 1;
			}

			return string.Compare(this.EscapedFullName, other.EscapedFullName);
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IComparable<DatabaseName>)this).CompareTo(obj as DatabaseName);
		}

	}

}
