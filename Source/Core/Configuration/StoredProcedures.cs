using System.Collections.Generic;



namespace SqlFramework.Configuration
{

	public sealed class StoredProcedures
	{

		[System.Xml.Serialization.XmlAttribute("Namespace")]
		public string Namespace { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("Procedure")]
		public List<StoredProcedureElement> Elements { get; set; }

	}



}
