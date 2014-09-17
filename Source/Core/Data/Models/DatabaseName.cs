namespace SqlFramework.Data.Models
{
    using System;

    public sealed class DatabaseName : IDatabaseName, IComparable<DatabaseName>, IComparable
    {
        public override string ToString()
        {
            return EscapedFullName;
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IComparable<DatabaseName>)this).CompareTo(obj as DatabaseName);
        }

        int IComparable<DatabaseName>.CompareTo(DatabaseName other)
        {
            if (other == null)
            {
                return 1;
            }

            return string.Compare(EscapedFullName, other.EscapedFullName, StringComparison.OrdinalIgnoreCase);
        }

        public string SchemaName { get; set; }

        public string Name { get; set; }

        public string EscapedSchemaName { get; set; }

        public string EscapedName { get; set; }

        public string EscapedFullName { get; set; }
    }
}