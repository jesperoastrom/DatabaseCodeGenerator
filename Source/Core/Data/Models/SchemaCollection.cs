namespace SqlFramework.Data.Models
{
    using System;
    using System.Collections.Generic;
    using Builders;

    public sealed class SchemaCollection<TElement>
    {
        public SchemaCollection(ISchemaElementCollectionBuilder collectionBuilder, string elementNamespace)
        {
            _collectionBuilder = collectionBuilder;
            ElementNamespace = elementNamespace;
            _schemas = new Dictionary<string, ISchemaElementCollection<TElement>>(StringComparer.OrdinalIgnoreCase);
        }

        public string ElementNamespace { get; private set; }

        public IEnumerable<ISchemaElementCollection<TElement>> SchemaElementCollections
        {
            get { return _schemas.Values; }
        }

        public void AddElement(string schemaName, TElement element)
        {
            if (_schemas.ContainsKey(schemaName))
            {
                _schemas[schemaName].Elements.Add(element);
            }
            else
            {
                var schemaElementCollection = _collectionBuilder.Build<TElement>(schemaName);
                schemaElementCollection.Elements.Add(element);
                _schemas.Add(schemaName, schemaElementCollection);
            }
        }

        private readonly ISchemaElementCollectionBuilder _collectionBuilder;
        private readonly Dictionary<string, ISchemaElementCollection<TElement>> _schemas;
    }
}