using System.Collections.Generic;

namespace SqlFramework.Data.Models
{
    public interface ISchemaElementCollection<TItem>
    {

        string SchemaName { get; }
        string StaticTypeName { get; }
        /// <summary>
        ///     Elements ordered by name.
        /// </summary>
        List<TItem> Elements { get; }
    }
}