using SqlFramework.Data.Models;

namespace SqlFramework.Data.Builders
{
    public interface ITypeNameBuilder
    {
        ITypeName Build(string ns, string name);
    }
}