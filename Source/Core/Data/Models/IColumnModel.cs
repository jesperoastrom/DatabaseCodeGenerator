namespace SqlFramework.Data.Models
{
    public interface IColumnModel
    {
        string DatabaseName { get; }
        string ParameterName { get; }
        string PropertyName { get; }
        ClrType ClrType { get; }
    }
}