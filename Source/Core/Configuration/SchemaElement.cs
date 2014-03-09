namespace SqlFramework.Configuration
{
    public abstract class SchemaElement
    {
        [System.Xml.Serialization.XmlAttribute("SchemaName")]
        public string SchemaName { get; set; }

        [System.Xml.Serialization.XmlAttribute("Name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return SchemaName + "." + Name;
        }
    }
}