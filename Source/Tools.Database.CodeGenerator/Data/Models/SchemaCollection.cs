using System;
using System.Collections.Generic;
using System.Linq;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	internal sealed class SchemaCollection<TElement>
	{

		public SchemaCollection(string elementNamespace)
		{
			this.ElementNamespace = elementNamespace;
			this.schemas = new Dictionary<string, SchemaElementCollection<TElement>>(StringComparer.OrdinalIgnoreCase);
		}


		public string ElementNamespace { get; private set; }

		public IEnumerable<SchemaElementCollection<TElement>> SchemaElementCollections
		{
			get
			{
				return this.schemas.Values;
			}
		}



		public void AddElement(string schemaName, TElement element)
		{
			if(this.schemas.ContainsKey(schemaName))
			{
				this.schemas[schemaName].Elements.Add(element);
			}
			else
			{
				var schemaElementCollection = new SchemaElementCollection<TElement>(schemaName);
				schemaElementCollection.Elements.Add(element);
				this.schemas.Add(schemaName, schemaElementCollection);
			}
		}




		private readonly Dictionary<string, SchemaElementCollection<TElement>> schemas;

	}

}
