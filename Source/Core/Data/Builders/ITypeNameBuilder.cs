namespace SqlFramework.Data.Builders
{
    using Models;

    public interface ITypeNameBuilder
    {
        ITypeName Build(string ns, string name);
    }
}