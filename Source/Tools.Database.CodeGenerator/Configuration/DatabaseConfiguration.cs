


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

}
