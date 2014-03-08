namespace SqlFramework.Data
{
    public sealed class ConnectionStringProvider : IConnectionStringProvider
    {
        public ConnectionStringProvider(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }
    }
}