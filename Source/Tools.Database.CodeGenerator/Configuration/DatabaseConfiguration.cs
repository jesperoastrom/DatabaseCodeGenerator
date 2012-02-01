using System.Collections.Generic;



namespace Flip.Tools.Database.CodeGenerator.Configuration
{

	[System.Xml.Serialization.XmlRoot("Database")]
	public sealed class DatabaseConfiguration
	{

		[System.Xml.Serialization.XmlElement("StoredProcedures", typeof(StoredProcedures))]
		public StoredProcedures StoredProcedures { get; set; }

		[System.Xml.Serialization.XmlElement("UserDefinedTypes", typeof(UserDefinedTypes))]
		public UserDefinedTypes UserDefinedTypes { get; set; }

	}

	internal static class StringExtensions
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

	public sealed class StoredProcedures
	{

		[System.Xml.Serialization.XmlAttribute("Namespace")]
		public string Namespace { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("Procedure")]
		public List<StoredProcedureElement> Elements { get; set; }

	}

	public sealed class UserDefinedTypes
	{

		[System.Xml.Serialization.XmlAttribute("Namespace")]
		public string Namespace { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("Type")]
		public List<UserDefinedTypeElement> Elements { get; set; }

	}

	public sealed class StoredProcedureElement : SchemaElement
	{
	}

	public sealed class UserDefinedTypeElement : SchemaElement
	{
	}

}
