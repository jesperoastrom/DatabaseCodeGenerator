namespace SqlFramework
{
    using System.Configuration;

    public static class TestConnectionString
    {
        public static readonly string Value = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
    }
}
