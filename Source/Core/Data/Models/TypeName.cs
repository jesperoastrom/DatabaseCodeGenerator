using System;

namespace SqlFramework.Data.Models
{
    public sealed class TypeName : ITypeName, IComparable<TypeName>, IComparable
    {
        public override string ToString()
        {
            return FullyQualifiedTypeName;
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IComparable<TypeName>) this).CompareTo(obj as TypeName);
        }

        int IComparable<TypeName>.CompareTo(TypeName other)
        {
            if (other == null)
            {
                return 1;
            }

            return string.Compare(FullyQualifiedTypeName, other.FullyQualifiedTypeName);
        }

        public string Namespace { get; set; }

        public string Name { get; set; }

        public string FullyQualifiedTypeName { get; set; }
    }
}