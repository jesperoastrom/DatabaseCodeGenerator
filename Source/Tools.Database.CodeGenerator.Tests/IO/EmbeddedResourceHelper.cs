using System.IO;
using System.Reflection;



namespace Flip.Tools.Database.CodeGenerator.Tests.IO
{

	internal static class EmbeddedResourceHelper
	{

		public static string GetStringFromEmbeddedResource(string resourceName)
		{
			string script;

			using (Stream stream = GetStreamFromEmbeddedResource(resourceName))
			{
				using (var reader = new StreamReader(stream))
				{
					script = reader.ReadToEnd();
				}
			}
			return script;
		}


		public static Stream GetStreamFromEmbeddedResource(string resourceName)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			return assembly.GetManifestResourceStream(resourceName);
		}

	}

}
