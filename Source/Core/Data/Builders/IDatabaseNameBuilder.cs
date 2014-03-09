using SqlFramework.Data.Models;

namespace SqlFramework.Data.Builders
{
    public interface IDatabaseNameBuilder
    {
        IDatabaseName Build(string schema, string name);
    }
}
