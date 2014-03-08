namespace SqlFramework.Configuration
{
    public abstract class SchemaElement
    {
        [System.Xml.Serialization.XmlAttribute("SchemaName")]
        public string SchemaName
        {
            get { return _schemaName; }
            set
            {
                _schemaName = value;
                EscapedSchemaName = value.EscapeDatabaseName();
                EscapedFullName = EscapedSchemaName + "." + EscapedName;
            }
        }

        [System.Xml.Serialization.XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                EscapedName = value.EscapeDatabaseName();
                EscapedFullName = EscapedSchemaName + "." + EscapedName;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string EscapedSchemaName { get; private set; }

        [System.Xml.Serialization.XmlIgnore]
        public string EscapedName { get; private set; }

        [System.Xml.Serialization.XmlIgnore]
        public string EscapedFullName { get; private set; }

        public override string ToString()
        {
            return EscapedFullName;
        }

        private string _name;
        private string _schemaName;
    }
}