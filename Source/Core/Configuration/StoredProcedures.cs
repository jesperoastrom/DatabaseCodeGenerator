namespace SqlFramework.Configuration
{
    using System.Collections.Generic;

    public sealed class StoredProcedures
    {
        [System.Xml.Serialization.XmlAttribute("Namespace")]
        public string Namespace { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Procedure")]
        public List<StoredProcedureElement> Elements { get; set; }
    }
}