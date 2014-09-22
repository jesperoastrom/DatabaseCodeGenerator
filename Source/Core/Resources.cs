namespace SqlFramework
{
    internal class Resources
    {
        public static class Configuration
        {
            public const string DatabaseConfigurationXsd = ConfigurationPath + "DatabaseConfiguration.xsd";
            private const string ConfigurationPath = RootPath + "Configuration.";
        }

        private const string RootPath = "SqlFramework.";
    }
}