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

	}

}
