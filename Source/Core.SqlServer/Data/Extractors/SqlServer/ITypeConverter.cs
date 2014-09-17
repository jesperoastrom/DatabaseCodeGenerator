namespace SqlFramework.Data.Extractors.SqlServer
{
    using System;
    using System.Data;
    using Microsoft.SqlServer.Management.Smo;
    using Models;

    public interface ITypeConverter
    {
        ClrType ToClrType(StoredProcedureParameter parameter, string ns);

        ClrType ToClrType(Column column, string ns);

        ClrType ToClrType(Type type, bool nullable);

        SqlDbType ToSqlDbDataType(DataType dataType);
    }
}