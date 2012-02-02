using System.Collections.Generic;



namespace Flip.Tools.Database.CodeGenerator.Configuration
{

	public sealed class UserDefinedTableTypes
	{

		[System.Xml.Serialization.XmlAttribute("Namespace")]
		public string Namespace { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("Type")]
		public List<UserDefinedTypeElement> Elements { get; set; }

	}

}
