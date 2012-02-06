using System.Collections.Generic;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	internal class SchemaElementCollection<TItem>
	{

		public SchemaElementCollection(string schemaName)
		{
			this.SchemaName = schemaName;
			this.StaticTypeName = schemaName.ToTypeName();
			this.Elements = new List<TItem>();
		}



		public string SchemaName { get; private set; }
		public string StaticTypeName { get; private set; }

		/// <summary>
		/// Elements ordered by name.
		/// </summary>
		public List<TItem> Elements { get; private set; }

	}

}
