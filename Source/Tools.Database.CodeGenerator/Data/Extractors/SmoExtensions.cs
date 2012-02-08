using System;
using System.Collections.Generic;
using Smo = Microsoft.SqlServer.Management.Smo;



namespace Flip.Tools.Database.CodeGenerator.Data.Extractors
{

	internal static class SmoExtensions
	{

		static SmoExtensions()
		{
			prettyTypeLook = new Dictionary<Type, string>();
			prettyTypeLook.Add(typeof(System.Byte), "byte");
			prettyTypeLook.Add(typeof(System.Int16), "short");
			prettyTypeLook.Add(typeof(System.Int32), "int");
			prettyTypeLook.Add(typeof(System.Int64), "long");
			prettyTypeLook.Add(typeof(System.Boolean), "bool");
			prettyTypeLook.Add(typeof(System.Decimal), "decimal");
			prettyTypeLook.Add(typeof(System.Char), "char");
			prettyTypeLook.Add(typeof(System.String), "string");

			sqlDataTypeLookup = new Dictionary<Smo.SqlDataType, Type>();

			sqlDataTypeLookup.Add(Smo.SqlDataType.BigInt, typeof(System.Int64));
			sqlDataTypeLookup.Add(Smo.SqlDataType.Bit, typeof(System.Boolean));
			sqlDataTypeLookup.Add(Smo.SqlDataType.Binary, typeof(System.Byte[]));

			sqlDataTypeLookup.Add(Smo.SqlDataType.Char, typeof(System.String));

			sqlDataTypeLookup.Add(Smo.SqlDataType.Date, typeof(System.DateTime));
			sqlDataTypeLookup.Add(Smo.SqlDataType.DateTime, typeof(System.DateTime));
			sqlDataTypeLookup.Add(Smo.SqlDataType.DateTime2, typeof(System.DateTime));
			sqlDataTypeLookup.Add(Smo.SqlDataType.DateTimeOffset, typeof(System.DateTimeOffset));
			sqlDataTypeLookup.Add(Smo.SqlDataType.Decimal, typeof(System.Decimal));

			sqlDataTypeLookup.Add(Smo.SqlDataType.Float, typeof(System.Double));

			sqlDataTypeLookup.Add(Smo.SqlDataType.Image, typeof(System.Byte[]));
			sqlDataTypeLookup.Add(Smo.SqlDataType.Int, typeof(System.Int32));

			sqlDataTypeLookup.Add(Smo.SqlDataType.Money, typeof(System.Decimal));

			sqlDataTypeLookup.Add(Smo.SqlDataType.NChar, typeof(System.String));
			sqlDataTypeLookup.Add(Smo.SqlDataType.NText, typeof(System.String));
			sqlDataTypeLookup.Add(Smo.SqlDataType.Numeric, typeof(System.Decimal));
			sqlDataTypeLookup.Add(Smo.SqlDataType.NVarChar, typeof(System.String));
			sqlDataTypeLookup.Add(Smo.SqlDataType.NVarCharMax, typeof(System.String));

			sqlDataTypeLookup.Add(Smo.SqlDataType.Real, typeof(System.Single));

			sqlDataTypeLookup.Add(Smo.SqlDataType.SmallDateTime, typeof(System.DateTime));
			sqlDataTypeLookup.Add(Smo.SqlDataType.SmallInt, typeof(System.Int16));
			sqlDataTypeLookup.Add(Smo.SqlDataType.SmallMoney, typeof(System.Decimal));
			sqlDataTypeLookup.Add(Smo.SqlDataType.SysName, typeof(System.String));

			sqlDataTypeLookup.Add(Smo.SqlDataType.Text, typeof(System.String));
			sqlDataTypeLookup.Add(Smo.SqlDataType.Time, typeof(System.TimeSpan));
			sqlDataTypeLookup.Add(Smo.SqlDataType.Timestamp, typeof(System.Byte[]));
			sqlDataTypeLookup.Add(Smo.SqlDataType.TinyInt, typeof(System.Byte));

			sqlDataTypeLookup.Add(Smo.SqlDataType.UniqueIdentifier, typeof(System.Guid));
			sqlDataTypeLookup.Add(Smo.SqlDataType.UserDefinedDataType, typeof(System.Object));
			sqlDataTypeLookup.Add(Smo.SqlDataType.UserDefinedTableType, typeof(System.Data.DataTable));
			sqlDataTypeLookup.Add(Smo.SqlDataType.UserDefinedType, typeof(System.Object));

			sqlDataTypeLookup.Add(Smo.SqlDataType.VarBinary, typeof(System.Byte[]));
			sqlDataTypeLookup.Add(Smo.SqlDataType.VarBinaryMax, typeof(System.Byte[]));
			sqlDataTypeLookup.Add(Smo.SqlDataType.VarChar, typeof(System.String));
			sqlDataTypeLookup.Add(Smo.SqlDataType.VarCharMax, typeof(System.String));
			sqlDataTypeLookup.Add(Smo.SqlDataType.Variant, typeof(System.Object));

			sqlDataTypeLookup.Add(Smo.SqlDataType.Xml, typeof(System.Xml.Linq.XElement));
		}



		public static string ToClrString(this Smo.DataType dataType)
		{
			return GetType(dataType.SqlDataType).ToClrString();
		}

		public static string ToClrString(this Type type)
		{
			if (prettyTypeLook.ContainsKey(type))
			{
				return prettyTypeLook[type];
			}
			if (type.Assembly.FullName == "System")
			{
				return type.Name;
			}
			return type.FullName;
		}



		private static Type GetType(Smo.SqlDataType sqlDataType)
		{
			if (sqlDataTypeLookup.ContainsKey(sqlDataType))
			{
				return sqlDataTypeLookup[sqlDataType];
			}
			throw new ArgumentException("Unknown sql type '" + sqlDataType.ToString() + "'.");
		}



		private static Dictionary<Smo.SqlDataType, Type> sqlDataTypeLookup;
		private static Dictionary<Type, string> prettyTypeLook;

	}

}
