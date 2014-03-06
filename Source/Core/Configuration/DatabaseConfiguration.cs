using System.Xml.Serialization;

namespace SqlFramework.Configuration
{
    [XmlRoot("Database")]
    public sealed class DatabaseConfiguration
    {
        [XmlElement("StoredProcedures", typeof (StoredProcedures))]
        public StoredProcedures StoredProcedures { get; set; }

        [XmlElement("UserDefinedTableTypes", typeof (UserDefinedTableTypes))]
        public UserDefinedTableTypes UserDefinedTableTypes { get; set; }

        [XmlIgnore()]
        public string TableTypeNamespaceFromStoredProcedure { get; set; }
    }
}