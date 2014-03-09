namespace SqlFramework.Data.Models
{
    public interface ITypeName
    {
        string Namespace { get; }

        string Name { get; }

        string FullyQualifiedTypeName { get; }
    }
}