using System;
using System.Collections.Generic;
using System.Data;
using SqlFramework.Data.Models;

namespace SqlFramework.Data.Extractors.SqlServer2012
{
    internal static class SmoExtensions
    {
        static SmoExtensions()
        {
            prettyStringLookup = new Dictionary<Type, string>();
            prettyStringLookup.Add(typeof (System.Byte), "byte");
            prettyStringLookup.Add(typeof (System.Int16), "short");
            prettyStringLookup.Add(typeof (System.Int32), "int");
            prettyStringLookup.Add(typeof (System.Int64), "long");
            prettyStringLookup.Add(typeof (System.Boolean), "bool");
            prettyStringLookup.Add(typeof (System.Decimal), "decimal");
            prettyStringLookup.Add(typeof (System.Char), "char");
            prettyStringLookup.Add(typeof (System.String), "string");

            typeLookup = new Dictionary<Microsoft.SqlServer.Management.Smo.SqlDataType, Type>();
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.BigInt, typeof (System.Int64));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Bit, typeof (System.Boolean));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Binary, typeof (System.Byte[]));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Char, typeof (System.String));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Date, typeof (System.DateTime));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.DateTime, typeof (System.DateTime));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.DateTime2, typeof (System.DateTime));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.DateTimeOffset, typeof (System.DateTimeOffset));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Decimal, typeof (System.Decimal));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Float, typeof (System.Double));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Image, typeof (System.Byte[]));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Int, typeof (System.Int32));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Money, typeof (System.Decimal));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.NChar, typeof (System.String));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.NText, typeof (System.String));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Numeric, typeof (System.Decimal));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.NVarChar, typeof (System.String));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.NVarCharMax, typeof (System.String));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Real, typeof (System.Single));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.SmallDateTime, typeof (System.DateTime));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.SmallInt, typeof (System.Int16));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.SmallMoney, typeof (System.Decimal));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.SysName, typeof (System.String));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Text, typeof (System.String));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Time, typeof (System.TimeSpan));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Timestamp, typeof (System.Byte[]));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.TinyInt, typeof (System.Byte));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.UniqueIdentifier, typeof (System.Guid));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.UserDefinedDataType, typeof (System.Object));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.UserDefinedTableType, typeof (System.Data.DataTable));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.UserDefinedType, typeof (System.Object));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.VarBinary, typeof (System.Byte[]));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.VarBinaryMax, typeof (System.Byte[]));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.VarChar, typeof (System.String));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.VarCharMax, typeof (System.String));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Variant, typeof (System.Object));
            typeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Xml, typeof (System.Xml.Linq.XElement));

            sqlDbTypeLookup = new Dictionary<Microsoft.SqlServer.Management.Smo.SqlDataType, SqlDbType>();
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.BigInt, SqlDbType.BigInt);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Bit, SqlDbType.Bit);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Binary, SqlDbType.Binary);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Char, SqlDbType.Char);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Date, SqlDbType.Date);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.DateTime, SqlDbType.DateTime);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.DateTime2, SqlDbType.DateTime2);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.DateTimeOffset, SqlDbType.DateTimeOffset);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Decimal, SqlDbType.Decimal);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Float, SqlDbType.Float);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Image, SqlDbType.Image);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Int, SqlDbType.Int);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Money, SqlDbType.Money);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.NChar, SqlDbType.NChar);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.NText, SqlDbType.NText);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Numeric, SqlDbType.Decimal);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.NVarChar, SqlDbType.NVarChar);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.NVarCharMax, SqlDbType.NVarChar);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Real, SqlDbType.Real);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.SmallDateTime, SqlDbType.SmallDateTime);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.SmallInt, SqlDbType.SmallInt);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.SmallMoney, SqlDbType.SmallMoney);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.SysName, SqlDbType.VarChar);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Text, SqlDbType.Text);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Time, SqlDbType.Time);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Timestamp, SqlDbType.Timestamp);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.TinyInt, SqlDbType.TinyInt);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.UniqueIdentifier, SqlDbType.UniqueIdentifier);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.UserDefinedDataType, SqlDbType.Udt);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.UserDefinedTableType, SqlDbType.Structured);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.UserDefinedType, SqlDbType.Udt);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.VarBinary, SqlDbType.VarBinary);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.VarBinaryMax, SqlDbType.VarBinary);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.VarChar, SqlDbType.VarChar);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.VarCharMax, SqlDbType.VarChar);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Variant, SqlDbType.Variant);
            sqlDbTypeLookup.Add(Microsoft.SqlServer.Management.Smo.SqlDataType.Xml, SqlDbType.Xml);
        }

        public static ClrType ToClrType(this Microsoft.SqlServer.Management.Smo.StoredProcedureParameter parameter, string ns)
        {
            if (parameter.DataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.UserDefinedTableType)
            {
                string typeName = TypeName.GetFullyQualifiedTypeName(ns, parameter.DataType.Schema, parameter.DataType.Name);
                return new ClrType()
                           {
                               IsUserDefined = true,
                               InnerTypeName = typeName,
                               TypeName = typeName
                           };
            }
            else
            {
                return ToClrType(GetType(parameter.DataType.SqlDataType), true);
            }
        }

        public static ClrType ToClrType(this Microsoft.SqlServer.Management.Smo.Column column, string ns)
        {
            if (column.DataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.UserDefinedTableType)
            {
                string typeName = TypeName.GetFullyQualifiedTypeName(ns, column.DataType.Schema, column.DataType.Name);
                return new ClrType()
                           {
                               IsUserDefined = true,
                               InnerTypeName = typeName,
                               TypeName = typeName
                           };
            }
            else
            {
                return GetType(column.DataType.SqlDataType).ToClrType(column.Nullable);
            }
        }

        public static ClrType ToClrType(this Type type, bool nullable)
        {
            var clrType = new ClrType();

            if (prettyStringLookup.ContainsKey(type))
            {
                clrType.TypeName = prettyStringLookup[type];
            }
            else if (type.Namespace == "System")
            {
                clrType.TypeName = type.Name;
            }
            else
            {
                clrType.TypeName = type.FullName;
            }

            clrType.InnerTypeName = clrType.TypeName;

            if (type.IsValueType && nullable)
            {
                clrType.TypeName = clrType.TypeName + "?";
                clrType.IsNullable = true;
            }

            return clrType;
        }

        public static SqlDbType ToSqlDbDataType(this Microsoft.SqlServer.Management.Smo.DataType dataType)
        {
            return sqlDbTypeLookup[dataType.SqlDataType];
        }

        private static Type GetType(Microsoft.SqlServer.Management.Smo.SqlDataType sqlDataType)
        {
            if (typeLookup.ContainsKey(sqlDataType))
            {
                return typeLookup[sqlDataType];
            }
            throw new ArgumentException("Unknown sql type '" + sqlDataType.ToString() + "'.");
        }

        private static readonly Dictionary<Microsoft.SqlServer.Management.Smo.SqlDataType, Type> typeLookup;
        private static readonly Dictionary<Microsoft.SqlServer.Management.Smo.SqlDataType, SqlDbType> sqlDbTypeLookup;
        private static readonly Dictionary<Type, string> prettyStringLookup;
    }
}