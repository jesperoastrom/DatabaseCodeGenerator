using System.Collections.Generic;

namespace SqlFramework.Data.Models
{
    public class SchemaElementCollection<TItem> : ISchemaElementCollection<TItem>
    {
        public SchemaElementCollection()
        {
            Elements = new List<TItem>();
        }
        public string SchemaName { get; set; }
        public string StaticTypeName { get; set; }
        public List<TItem> Elements { get; private set; }
    }
}