using System;



namespace Flip.Tools.Database.CodeGenerator
{

	internal static class StringExtensions
	{

		public static string EscapeDatabaseName(this string name)
		{
			if (name == null)
			{
				return name;
			}

			if (!name.StartsWith("["))
			{
				name = "[" + name;
			}
			if (!name.EndsWith("]"))
			{
				name = name + "]";
			}

			return name;
		}

		public static string ToTypeName(this string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			name = CreateValidIdentifier(name);

			if (name.Length == 1)
			{
				return name.ToUpper();
			}
			else
			{
				return char.ToUpper(name[0]) + name.Substring(1);
			}
		}

		public static string ToPropertyName(this string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			if (name.StartsWith("@"))
			{
				name = name.Substring(1);
			}

			return name.ToTypeName();
		}

		public static string ToParameterName(this string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			if(name.StartsWith("@"))
			{
				name = name.Substring(1);
			}

			name = CreateValidIdentifier(name);

			if (name.Length == 1)
			{
				return name.ToLower();
			}
			else
			{
				return char.ToLower(name[0]) + name.Substring(1);
			}
		}



		private static string CreateValidIdentifier(string value)
		{
			return codeProvider.CreateValidIdentifier(value);
		}



		//TODO Choose provider
		private static readonly Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();

	}

}
