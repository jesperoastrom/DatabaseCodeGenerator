using System.Collections.Generic;



namespace Flip.Tools.Database.CodeGenerator.Configuration
{

	[System.Xml.Serialization.XmlRoot("Database")]
	public sealed class DatabaseConfiguration
	{

		[System.Xml.Serialization.XmlArray("StoredProcedures")]
		[System.Xml.Serialization.XmlArrayItem("Procedure", typeof(StoredProcedureElement))]
		public List<StoredProcedureElement> StoredProcedures { get; set; }

		[System.Xml.Serialization.XmlArray("UserDefinedTypes")]
		[System.Xml.Serialization.XmlArrayItem("Type", typeof(UserDefinedTypeElement))]
		public List<UserDefinedTypeElement> UserDefinedTypes { get; set; }

	}

	public static class StringExtensions
	{

		public static string EscapeDatabaseName(this string name)
		{
			if (name == null)
			{
				return name;
			}

			if (!name.StartsWith("["))
			{
				name = "[" + name;
			}
			if (!name.EndsWith("]"))
			{
				name = name + "]";
			}

			return name;
		}

	}

	public abstract class SchemaElement
	{

		[System.Xml.Serialization.XmlAttribute("SchemaName")]
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

		[System.Xml.Serialization.XmlAttribute("Name")]
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

		[System.Xml.Serialization.XmlIgnore]
		public string EscapedSchemaName { get; private set; }

		[System.Xml.Serialization.XmlIgnore]
		public string EscapedName { get; private set; }

		[System.Xml.Serialization.XmlIgnore]
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

	public sealed class StoredProcedureElement : SchemaElement
	{
	}

	public sealed class UserDefinedTypeElement : SchemaElement
	{
	}

}
