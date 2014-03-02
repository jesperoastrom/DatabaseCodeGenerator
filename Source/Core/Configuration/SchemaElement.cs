namespace SqlFramework.Configuration
{

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

}
