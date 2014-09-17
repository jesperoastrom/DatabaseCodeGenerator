namespace SqlFramework.Data.Builders
{
    using Models;

    public interface IDatabaseNameBuilder
    {
        IDatabaseName Build(string schema, string name);
    }
}