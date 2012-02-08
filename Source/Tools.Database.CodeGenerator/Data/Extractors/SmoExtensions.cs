using System;
using System.Collections.Generic;
using System.Data;
using Smo = Microsoft.SqlServer.Management.Smo;
using Flip.Tools.Database.CodeGenerator.Data.Models;



namespace Flip.Tools.Database.CodeGenerator.Data.Extractors
{

	internal static class SmoExtensions
	{

		static SmoExtensions()
		{
			prettyStringLookup = new Dictionary<Type, string>();
			prettyStringLookup.Add(typeof(System.Byte), "byte");
			prettyStringLookup.Add(typeof(System.Int16), "short");
			prettyStringLookup.Add(typeof(System.Int32), "int");
			prettyStringLookup.Add(typeof(System.Int64), "long");
			prettyStringLookup.Add(typeof(System.Boolean), "bool");
			prettyStringLookup.Add(typeof(System.Decimal), "decimal");
			prettyStringLookup.Add(typeof(System.Char), "char");
			prettyStringLookup.Add(typeof(System.String), "string");



			typeLookup = new Dictionary<Smo.SqlDataType, Type>();
			typeLookup.Add(Smo.SqlDataType.BigInt, typeof(System.Int64));
			typeLookup.Add(Smo.SqlDataType.Bit, typeof(System.Boolean));
			typeLookup.Add(Smo.SqlDataType.Binary, typeof(System.Byte[]));
			typeLookup.Add(Smo.SqlDataType.Char, typeof(System.String));
			typeLookup.Add(Smo.SqlDataType.Date, typeof(System.DateTime));
			typeLookup.Add(Smo.SqlDataType.DateTime, typeof(System.DateTime));
			typeLookup.Add(Smo.SqlDataType.DateTime2, typeof(System.DateTime));
			typeLookup.Add(Smo.SqlDataType.DateTimeOffset, typeof(System.DateTimeOffset));
			typeLookup.Add(Smo.SqlDataType.Decimal, typeof(System.Decimal));
			typeLookup.Add(Smo.SqlDataType.Float, typeof(System.Double));
			typeLookup.Add(Smo.SqlDataType.Image, typeof(System.Byte[]));
			typeLookup.Add(Smo.SqlDataType.Int, typeof(System.Int32));
			typeLookup.Add(Smo.SqlDataType.Money, typeof(System.Decimal));
			typeLookup.Add(Smo.SqlDataType.NChar, typeof(System.String));
			typeLookup.Add(Smo.SqlDataType.NText, typeof(System.String));
			typeLookup.Add(Smo.SqlDataType.Numeric, typeof(System.Decimal));
			typeLookup.Add(Smo.SqlDataType.NVarChar, typeof(System.String));
			typeLookup.Add(Smo.SqlDataType.NVarCharMax, typeof(System.String));
			typeLookup.Add(Smo.SqlDataType.Real, typeof(System.Single));
			typeLookup.Add(Smo.SqlDataType.SmallDateTime, typeof(System.DateTime));
			typeLookup.Add(Smo.SqlDataType.SmallInt, typeof(System.Int16));
			typeLookup.Add(Smo.SqlDataType.SmallMoney, typeof(System.Decimal));
			typeLookup.Add(Smo.SqlDataType.SysName, typeof(System.String));
			typeLookup.Add(Smo.SqlDataType.Text, typeof(System.String));
			typeLookup.Add(Smo.SqlDataType.Time, typeof(System.TimeSpan));
			typeLookup.Add(Smo.SqlDataType.Timestamp, typeof(System.Byte[]));
			typeLookup.Add(Smo.SqlDataType.TinyInt, typeof(System.Byte));
			typeLookup.Add(Smo.SqlDataType.UniqueIdentifier, typeof(System.Guid));
			typeLookup.Add(Smo.SqlDataType.UserDefinedDataType, typeof(System.Object));
			typeLookup.Add(Smo.SqlDataType.UserDefinedTableType, typeof(System.Data.DataTable));
			typeLookup.Add(Smo.SqlDataType.UserDefinedType, typeof(System.Object));
			typeLookup.Add(Smo.SqlDataType.VarBinary, typeof(System.Byte[]));
			typeLookup.Add(Smo.SqlDataType.VarBinaryMax, typeof(System.Byte[]));
			typeLookup.Add(Smo.SqlDataType.VarChar, typeof(System.String));
			typeLookup.Add(Smo.SqlDataType.VarCharMax, typeof(System.String));
			typeLookup.Add(Smo.SqlDataType.Variant, typeof(System.Object));
			typeLookup.Add(Smo.SqlDataType.Xml, typeof(System.Xml.Linq.XElement));



			sqlDbTypeLookup = new Dictionary<Smo.SqlDataType, SqlDbType>();
			sqlDbTypeLookup.Add(Smo.SqlDataType.BigInt, SqlDbType.BigInt);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Bit, SqlDbType.Bit);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Binary, SqlDbType.Binary);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Char, SqlDbType.Char);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Date, SqlDbType.Date);
			sqlDbTypeLookup.Add(Smo.SqlDataType.DateTime, SqlDbType.DateTime);
			sqlDbTypeLookup.Add(Smo.SqlDataType.DateTime2, SqlDbType.DateTime2);
			sqlDbTypeLookup.Add(Smo.SqlDataType.DateTimeOffset, SqlDbType.DateTimeOffset);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Decimal, SqlDbType.Decimal);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Float, SqlDbType.Float);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Image, SqlDbType.Image);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Int, SqlDbType.Int);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Money, SqlDbType.Money);
			sqlDbTypeLookup.Add(Smo.SqlDataType.NChar, SqlDbType.NChar);
			sqlDbTypeLookup.Add(Smo.SqlDataType.NText, SqlDbType.NText);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Numeric, SqlDbType.Decimal);
			sqlDbTypeLookup.Add(Smo.SqlDataType.NVarChar, SqlDbType.NVarChar);
			sqlDbTypeLookup.Add(Smo.SqlDataType.NVarCharMax, SqlDbType.NVarChar);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Real, SqlDbType.Real);
			sqlDbTypeLookup.Add(Smo.SqlDataType.SmallDateTime, SqlDbType.SmallDateTime);
			sqlDbTypeLookup.Add(Smo.SqlDataType.SmallInt, SqlDbType.SmallInt);
			sqlDbTypeLookup.Add(Smo.SqlDataType.SmallMoney, SqlDbType.SmallMoney);
			sqlDbTypeLookup.Add(Smo.SqlDataType.SysName, SqlDbType.VarChar);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Text, SqlDbType.Text);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Time, SqlDbType.Time);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Timestamp, SqlDbType.Timestamp);
			sqlDbTypeLookup.Add(Smo.SqlDataType.TinyInt, SqlDbType.TinyInt);
			sqlDbTypeLookup.Add(Smo.SqlDataType.UniqueIdentifier, SqlDbType.UniqueIdentifier);
			sqlDbTypeLookup.Add(Smo.SqlDataType.UserDefinedDataType, SqlDbType.Udt);
			sqlDbTypeLookup.Add(Smo.SqlDataType.UserDefinedTableType, SqlDbType.Udt);
			sqlDbTypeLookup.Add(Smo.SqlDataType.UserDefinedType, SqlDbType.Udt);
			sqlDbTypeLookup.Add(Smo.SqlDataType.VarBinary, SqlDbType.VarBinary);
			sqlDbTypeLookup.Add(Smo.SqlDataType.VarBinaryMax, SqlDbType.VarBinary);
			sqlDbTypeLookup.Add(Smo.SqlDataType.VarChar, SqlDbType.VarChar);
			sqlDbTypeLookup.Add(Smo.SqlDataType.VarCharMax, SqlDbType.VarChar);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Variant, SqlDbType.Variant);
			sqlDbTypeLookup.Add(Smo.SqlDataType.Xml, SqlDbType.Xml);
		}



		public static string ToClrString(this Smo.DataType dataType, string ns)
		{
			if (dataType.SqlDataType == Smo.SqlDataType.UserDefinedTableType)
			{
				return TypeName.GetFullyQualifiedTypeName(ns, dataType.Schema, dataType.Name);
			}
			else
			{
				return GetType(dataType.SqlDataType).ToClrString();
			}
		}

		public static SqlDbType ToSqlDbDataType(this Smo.DataType dataType)
		{
			return sqlDbTypeLookup[dataType.SqlDataType];
		}

		public static string ToClrString(this Type type)
		{
			if (prettyStringLookup.ContainsKey(type))
			{
				return prettyStringLookup[type];
			}
			if (type.Assembly.FullName == "System")
			{
				return type.Name;
			}
			return type.FullName;
		}



		private static Type GetType(Smo.SqlDataType sqlDataType)
		{
			if (typeLookup.ContainsKey(sqlDataType))
			{
				return typeLookup[sqlDataType];
			}
			throw new ArgumentException("Unknown sql type '" + sqlDataType.ToString() + "'.");
		}



		private static Dictionary<Smo.SqlDataType, Type> typeLookup;
		private static Dictionary<Smo.SqlDataType, SqlDbType> sqlDbTypeLookup;
		private static Dictionary<Type, string> prettyStringLookup;

	}

}
