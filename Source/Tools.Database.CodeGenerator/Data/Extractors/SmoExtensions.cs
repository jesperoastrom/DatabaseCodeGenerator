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

			sqlTypeLookup = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
			sqlTypeLookup.Add("bit", typeof(System.Boolean));
			sqlTypeLookup.Add("tinyint", typeof(System.Int16));
			sqlTypeLookup.Add("int", typeof(System.Int32));
			sqlTypeLookup.Add("bigint", typeof(System.Int64));
			sqlTypeLookup.Add("smallmoney", typeof(System.Decimal));
			sqlTypeLookup.Add("money", typeof(System.Decimal));
			sqlTypeLookup.Add("decimal", typeof(System.Decimal));
			sqlTypeLookup.Add("numeric", typeof(System.Decimal));
			sqlTypeLookup.Add("real", typeof(System.Single));
			sqlTypeLookup.Add("float", typeof(System.Double));

			sqlTypeLookup.Add("char", typeof(System.String));
			sqlTypeLookup.Add("nchar", typeof(System.String));
			sqlTypeLookup.Add("varchar", typeof(System.String));
			sqlTypeLookup.Add("nvarchar", typeof(System.String));
			sqlTypeLookup.Add("text", typeof(System.String));
			sqlTypeLookup.Add("ntext", typeof(System.String));
			sqlTypeLookup.Add("xml", typeof(System.Xml.Linq.XElement));

			sqlTypeLookup.Add("smalldatetime", typeof(System.DateTime));
			sqlTypeLookup.Add("datetime", typeof(System.DateTime));
			sqlTypeLookup.Add("datetime2", typeof(System.DateTime));
			sqlTypeLookup.Add("datetimeoffset", typeof(System.DateTimeOffset));
			sqlTypeLookup.Add("date", typeof(System.DateTime));
			sqlTypeLookup.Add("time", typeof(System.TimeSpan));

			sqlTypeLookup.Add("binary", typeof(System.Byte[]));
			sqlTypeLookup.Add("varbinary", typeof(System.Byte[]));
			sqlTypeLookup.Add("image", typeof(System.Byte[]));
			sqlTypeLookup.Add("timestamp", typeof(System.Byte[]));

			sqlTypeLookup.Add("uniqueidentifier", typeof(System.Guid));
			sqlTypeLookup.Add("sql_variant", typeof(System.Object));
		}



		public static string ToClrString(this Smo.DataType dataType)
		{
			Type type = GetType(dataType.ToString());
			return type.ToClrString();
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
			return type.AssemblyQualifiedName;
		}



		private static Type GetType(string sqlType)
		{
			if (sqlTypeLookup.ContainsKey(sqlType))
			{
				return sqlTypeLookup[sqlType];
			}
			throw new ArgumentException("Unknown sql type '" + sqlType + "'.");
		}



		private static Dictionary<string, Type> sqlTypeLookup;
		private static Dictionary<Type, string> prettyTypeLook;

	}

}
