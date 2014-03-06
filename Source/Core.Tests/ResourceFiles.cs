namespace SqlFramework
{
    public static class ResourceFiles
    {
        public static class Configuration
        {
            public static class Tests
            {
                public static class Resources
                {
                    private const string Path = Tests.Path + "Resources.";
                    public const string Configuration_xml = Path + "Configuration.xml";
                }

                private const string Path = Configuration.Path + "Tests.";
            }

            private const string Path = ResourceFiles.Path + "Configuration.";
        }

        private const string Path = "SqlFramework.";
    }
}