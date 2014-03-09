using SqlFramework.Data.Models;

namespace SqlFramework.Data.Builders
{
    public sealed class TypeNameBuilder : ITypeNameBuilder
    {
        public TypeNameBuilder(IDatabaseToCodeNameConverter nameConverter)
        {
            _nameConverter = nameConverter;
        }

        public ITypeName Build(string ns, string name)
        {
            return new TypeName
                       {
                           Namespace = ns,
                           Name = _nameConverter.ToTypeName(name),
                           FullyQualifiedTypeName = _nameConverter.GetFullyQualifiedTypeName(ns, name)
                       };
        }

        private readonly IDatabaseToCodeNameConverter _nameConverter;
    }
}