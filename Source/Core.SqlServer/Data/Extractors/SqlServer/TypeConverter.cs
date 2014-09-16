using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using Microsoft.SqlServer.Management.Smo;
using SqlFramework.Data.Models;

namespace SqlFramework.Data.Extractors.SqlServer
{
    public sealed class TypeConverter : ITypeConverter
    {
        static TypeConverter()
        {
            PrettyStringLookup = new Dictionary<Type, string>();
            PrettyStringLookup.Add(typeof (Byte), "byte");
            PrettyStringLookup.Add(typeof (Int16), "short");
            PrettyStringLookup.Add(typeof (Int32), "int");
            PrettyStringLookup.Add(typeof (Int64), "long");
            PrettyStringLookup.Add(typeof (Boolean), "bool");
            PrettyStringLookup.Add(typeof (Decimal), "decimal");
            PrettyStringLookup.Add(typeof (Char), "char");
            PrettyStringLookup.Add(typeof (String), "string");

            TypeLookup = new Dictionary<SqlDataType, Type>();
            TypeLookup.Add(SqlDataType.BigInt, typeof (Int64));
            TypeLookup.Add(SqlDataType.Bit, typeof (Boolean));
            TypeLookup.Add(SqlDataType.Binary, typeof (Byte[]));
            TypeLookup.Add(SqlDataType.Char, typeof (String));
            TypeLookup.Add(SqlDataType.Date, typeof (DateTime));
            TypeLookup.Add(SqlDataType.DateTime, typeof (DateTime));
            TypeLookup.Add(SqlDataType.DateTime2, typeof (DateTime));
            TypeLookup.Add(SqlDataType.DateTimeOffset, typeof (DateTimeOffset));
            TypeLookup.Add(SqlDataType.Decimal, typeof (Decimal));
            TypeLookup.Add(SqlDataType.Float, typeof (Double));
            TypeLookup.Add(SqlDataType.Image, typeof (Byte[]));
            TypeLookup.Add(SqlDataType.Int, typeof (Int32));
            TypeLookup.Add(SqlDataType.Money, typeof (Decimal));
            TypeLookup.Add(SqlDataType.NChar, typeof (String));
            TypeLookup.Add(SqlDataType.NText, typeof (String));
            TypeLookup.Add(SqlDataType.Numeric, typeof (Decimal));
            TypeLookup.Add(SqlDataType.NVarChar, typeof (String));
            TypeLookup.Add(SqlDataType.NVarCharMax, typeof (String));
            TypeLookup.Add(SqlDataType.Real, typeof (Single));
            TypeLookup.Add(SqlDataType.SmallDateTime, typeof (DateTime));
            TypeLookup.Add(SqlDataType.SmallInt, typeof (Int16));
            TypeLookup.Add(SqlDataType.SmallMoney, typeof (Decimal));
            TypeLookup.Add(SqlDataType.SysName, typeof (String));
            TypeLookup.Add(SqlDataType.Text, typeof (String));
            TypeLookup.Add(SqlDataType.Time, typeof (TimeSpan));
            TypeLookup.Add(SqlDataType.Timestamp, typeof (Byte[]));
            TypeLookup.Add(SqlDataType.TinyInt, typeof (Byte));
            TypeLookup.Add(SqlDataType.UniqueIdentifier, typeof (Guid));
            TypeLookup.Add(SqlDataType.UserDefinedDataType, typeof (Object));
            TypeLookup.Add(SqlDataType.UserDefinedTableType, typeof (DataTable));
            TypeLookup.Add(SqlDataType.UserDefinedType, typeof (Object));
            TypeLookup.Add(SqlDataType.VarBinary, typeof (Byte[]));
            TypeLookup.Add(SqlDataType.VarBinaryMax, typeof (Byte[]));
            TypeLookup.Add(SqlDataType.VarChar, typeof (String));
            TypeLookup.Add(SqlDataType.VarCharMax, typeof (String));
            TypeLookup.Add(SqlDataType.Variant, typeof (Object));
            TypeLookup.Add(SqlDataType.Xml, typeof (XElement));

            SqlDbTypeLookup = new Dictionary<SqlDataType, SqlDbType>();
            SqlDbTypeLookup.Add(SqlDataType.BigInt, SqlDbType.BigInt);
            SqlDbTypeLookup.Add(SqlDataType.Bit, SqlDbType.Bit);
            SqlDbTypeLookup.Add(SqlDataType.Binary, SqlDbType.Binary);
            SqlDbTypeLookup.Add(SqlDataType.Char, SqlDbType.Char);
            SqlDbTypeLookup.Add(SqlDataType.Date, SqlDbType.Date);
            SqlDbTypeLookup.Add(SqlDataType.DateTime, SqlDbType.DateTime);
            SqlDbTypeLookup.Add(SqlDataType.DateTime2, SqlDbType.DateTime2);
            SqlDbTypeLookup.Add(SqlDataType.DateTimeOffset, SqlDbType.DateTimeOffset);
            SqlDbTypeLookup.Add(SqlDataType.Decimal, SqlDbType.Decimal);
            SqlDbTypeLookup.Add(SqlDataType.Float, SqlDbType.Float);
            SqlDbTypeLookup.Add(SqlDataType.Image, SqlDbType.Image);
            SqlDbTypeLookup.Add(SqlDataType.Int, SqlDbType.Int);
            SqlDbTypeLookup.Add(SqlDataType.Money, SqlDbType.Money);
            SqlDbTypeLookup.Add(SqlDataType.NChar, SqlDbType.NChar);
            SqlDbTypeLookup.Add(SqlDataType.NText, SqlDbType.NText);
            SqlDbTypeLookup.Add(SqlDataType.Numeric, SqlDbType.Decimal);
            SqlDbTypeLookup.Add(SqlDataType.NVarChar, SqlDbType.NVarChar);
            SqlDbTypeLookup.Add(SqlDataType.NVarCharMax, SqlDbType.NVarChar);
            SqlDbTypeLookup.Add(SqlDataType.Real, SqlDbType.Real);
            SqlDbTypeLookup.Add(SqlDataType.SmallDateTime, SqlDbType.SmallDateTime);
            SqlDbTypeLookup.Add(SqlDataType.SmallInt, SqlDbType.SmallInt);
            SqlDbTypeLookup.Add(SqlDataType.SmallMoney, SqlDbType.SmallMoney);
            SqlDbTypeLookup.Add(SqlDataType.SysName, SqlDbType.VarChar);
            SqlDbTypeLookup.Add(SqlDataType.Text, SqlDbType.Text);
            SqlDbTypeLookup.Add(SqlDataType.Time, SqlDbType.Time);
            SqlDbTypeLookup.Add(SqlDataType.Timestamp, SqlDbType.Timestamp);
            SqlDbTypeLookup.Add(SqlDataType.TinyInt, SqlDbType.TinyInt);
            SqlDbTypeLookup.Add(SqlDataType.UniqueIdentifier, SqlDbType.UniqueIdentifier);
            SqlDbTypeLookup.Add(SqlDataType.UserDefinedDataType, SqlDbType.Udt);
            SqlDbTypeLookup.Add(SqlDataType.UserDefinedTableType, SqlDbType.Structured);
            SqlDbTypeLookup.Add(SqlDataType.UserDefinedType, SqlDbType.Udt);
            SqlDbTypeLookup.Add(SqlDataType.VarBinary, SqlDbType.VarBinary);
            SqlDbTypeLookup.Add(SqlDataType.VarBinaryMax, SqlDbType.VarBinary);
            SqlDbTypeLookup.Add(SqlDataType.VarChar, SqlDbType.VarChar);
            SqlDbTypeLookup.Add(SqlDataType.VarCharMax, SqlDbType.VarChar);
            SqlDbTypeLookup.Add(SqlDataType.Variant, SqlDbType.Variant);
            SqlDbTypeLookup.Add(SqlDataType.Xml, SqlDbType.Xml);
        }

        public TypeConverter(IDatabaseToCodeNameConverter nameConverter)
        {
            _nameConverter = nameConverter;
        }

        public ClrType ToClrType(StoredProcedureParameter parameter, string ns)
        {
            if (parameter.DataType.SqlDataType == SqlDataType.UserDefinedTableType)
            {
                string typeName = _nameConverter.GetFullyQualifiedTypeName(ns, parameter.DataType.Schema,
                                                                           parameter.DataType.Name);
                return new ClrType
                           {
                               IsUserDefined = true,
                               InnerTypeName = typeName,
                               TypeName = typeName
                           };
            }
            return ToClrType(GetType(parameter.DataType.SqlDataType), true);
        }

        public ClrType ToClrType(Column column, string ns)
        {
            if (column.DataType.SqlDataType == SqlDataType.UserDefinedTableType)
            {
                string typeName = _nameConverter.GetFullyQualifiedTypeName(ns, column.DataType.Schema,
                                                                           column.DataType.Name);
                return new ClrType
                           {
                               IsUserDefined = true,
                               InnerTypeName = typeName,
                               TypeName = typeName
                           };
            }
            return ToClrType(GetType(column.DataType.SqlDataType), column.Nullable);
        }

        public ClrType ToClrType(Type type, bool nullable)
        {
            var clrType = new ClrType();

            if (PrettyStringLookup.ContainsKey(type))
            {
                clrType.TypeName = PrettyStringLookup[type];
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

        public SqlDbType ToSqlDbDataType(DataType dataType)
        {
            return SqlDbTypeLookup[dataType.SqlDataType];
        }

        private static Type GetType(SqlDataType sqlDataType)
        {
            if (TypeLookup.ContainsKey(sqlDataType))
            {
                return TypeLookup[sqlDataType];
            }
            throw new ArgumentException("Unknown sql type '" + sqlDataType + "'.");
        }

        private static readonly Dictionary<SqlDataType, Type> TypeLookup;
        private static readonly Dictionary<SqlDataType, SqlDbType> SqlDbTypeLookup;
        private static readonly Dictionary<Type, string> PrettyStringLookup;
        private readonly IDatabaseToCodeNameConverter _nameConverter;
    }
}