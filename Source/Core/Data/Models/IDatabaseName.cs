namespace SqlFramework.Data.Models
{
    public interface IDatabaseName
    {
        string SchemaName { get; }
        string Name { get; }
        string EscapedSchemaName { get; }
        string EscapedName { get; }
        string EscapedFullName { get; }
    }
}