namespace SqlFramework
{
    using System.IO;
    using System.Reflection;

    public static class EmbeddedResourceHelper
    {
        public static Stream GetStreamFromEmbeddedResource<TAssembly>(string resourceName)
        {
            Assembly assembly = typeof(TAssembly).Assembly;
            return assembly.GetManifestResourceStream(resourceName);
        }

        public static string GetStringFromEmbeddedResource<TAssembly>(string resourceName)
        {
            string script;

            using (Stream stream = GetStreamFromEmbeddedResource<TAssembly>(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    script = reader.ReadToEnd();
                }
            }
            return script;
        }
    }
}