using System;
using Microsoft.CSharp;

namespace SqlFramework.Data.SqlServer2012
{
    public sealed class SqlServerCSharpCodeNameConverter : IDatabaseToCodeNameConverter
    {
        public string EscapeDatabaseName(params string[] names)
        {
            return string.Join(".", names.ForEach(name => EscapeDatabaseName(name)));
        }

        public string EscapeDatabaseName(string name)
        {
            AssertNotNullOrEmpty(name, "name");

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

        public string GetShortestNamespaceTo(string fromNs, string toNs)
        {
            AssertNotNullOrEmpty(fromNs, "fromNs");
            AssertNotNullOrEmpty(toNs, "toNs");

            string[] fromParts = fromNs.Split('.');
            string[] toParts = toNs.Split('.');

            for (int i = 0; i < toParts.Length; i++)
            {
                if (fromParts[i] != toParts[i])
                {
                    var resultParts = new string[toParts.Length - i];
                    Array.Copy(toParts, i, resultParts, 0, resultParts.Length);
                    return string.Join(".", resultParts);
                }
            }
            return string.Empty;
        }

        public string ToParameterName(string name)
        {
            AssertNotNullOrEmpty(name, "name");

            if (name.StartsWith("@"))
            {
                name = name.Substring(1);
            }

            name = CreateValidIdentifier(name);

            if (name.Length == 1)
            {
                return name.ToLower();
            }
            return char.ToLower(name[0]) + name.Substring(1);
        }

        public string ToPropertyName(string name)
        {
            AssertNotNullOrEmpty(name, "name");

            if (name.StartsWith("@"))
            {
                name = name.Substring(1);
            }

            return ToTypeName(name);
        }

        public string ToTypeName(string name)
        {
            AssertNotNullOrEmpty(name, "name");

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

        public string GetFullyQualifiedTypeName(string ns, string schemaName, string name)
        {
            if (string.IsNullOrEmpty(ns))
            {
                return string.Join(".", ToTypeName(schemaName), ToTypeName(name));
            }
            return string.Join(".", ns, ToTypeName(schemaName), ToTypeName(name));
        }

        public string GetFullyQualifiedTypeName(string ns, string name)
        {
            if (string.IsNullOrEmpty(ns))
            {
                return ToTypeName(name);
            }
            return string.Join(".", ns, ToTypeName(name));
        }

        private static void AssertNotNullOrEmpty(string parameterValue, string parameterName)
        {
            if (parameterValue == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            if (string.IsNullOrEmpty(parameterValue))
            {
                throw new ArgumentException(parameterName);
            }
        }

        private static string CreateValidIdentifier(string value)
        {
            return CodeProvider.CreateValidIdentifier(value);
        }
        private static readonly CSharpCodeProvider CodeProvider = new CSharpCodeProvider();
    }
}