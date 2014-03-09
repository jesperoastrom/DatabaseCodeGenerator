namespace SqlFramework
{
    public interface IDatabaseToCodeNameConverter
    {
        string EscapeDatabaseName(params string[] names);
        string EscapeDatabaseName(string name);
        string GetFullyQualifiedTypeName(string ns, string schemaName, string name);
        string GetFullyQualifiedTypeName(string ns, string name);
        string GetShortestNamespaceTo(string fromNs, string toNs);
        string ToParameterName(string name);
        string ToPropertyName(string name);
        string ToTypeName(string name);
    }
}