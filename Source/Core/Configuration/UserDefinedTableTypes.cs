using System.Collections.Generic;

namespace SqlFramework.Configuration
{
    public sealed class UserDefinedTableTypes
    {
        [System.Xml.Serialization.XmlAttribute("Namespace")]
        public string Namespace { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Type")]
        public List<UserDefinedTypeTableElement> Elements { get; set; }
    }
}