namespace SqlFramework
{
    public class TestResources
    {
        public static class Configuration
        {
            public static class Tests
            {
                public static class Resources
                {
                    public const string DatabaseConfiguration_xml = Path + "DatabaseConfiguration.xml";
                    public const string InvalidDatabaseConfiguration_xml = Path + "InvalidDatabaseConfiguration.xml";
                    private const string Path = Tests.Path + "Resources.";
                }

                private const string Path = Configuration.Path + "Tests.";
            }

            private const string Path = TestResources.Path + "Configuration.";
        }

        private const string Path = "SqlFramework.";
    }
}