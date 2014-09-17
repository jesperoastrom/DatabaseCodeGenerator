namespace SqlFramework.Data.Models
{
    using System.Collections.Generic;

    public interface ISchemaElementCollection<TItem>
    {
        string SchemaName { get; }

        string StaticTypeName { get; }

        /// <summary>
        ///   Elements ordered by name.
        /// </summary>
        List<TItem> Elements { get; }
    }
}