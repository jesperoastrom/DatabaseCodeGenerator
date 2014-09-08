using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SqlFramework.Configuration;
using SqlFramework.Data.Models;

namespace SqlFramework.Data.SqlServer2012.Extractors
{
    public sealed class DatabaseExtractor : IDatabaseExtractor
    {
        public DatabaseExtractor(IConnectionStringProvider connectionStringProvider,
                                 IStoredProcedureExtractor storedProcedureExtractor)
        {
            this._connectionStringProvider = connectionStringProvider;
            _storedProcedureExtractor = storedProcedureExtractor;
        }

        private ConnectionDetails GetConnectionDetails()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionStringProvider.ConnectionString))
                {
                    connection.Open();
                    return new ConnectionDetails
                               {
                                   DataSource = connection.DataSource,
                                   Database = connection.Database
                               };
                }
            }
            catch (SqlException sqlException)
            {
                throw new InvalidArgumentException(string.Format("Could not connect to database using connection string '{0}'.", _connectionStringProvider.ConnectionString), sqlException);
            }
        }

        public DatabaseModel Extract(DatabaseConfiguration configuration)
        {
            ConnectionDetails connectionDetails = GetConnectionDetails();

            var serverConnection = new ServerConnection(connectionDetails.DataSource);
            var server = new Server(serverConnection);

            try
            {
                server.ConnectionContext.Connect();

                Database database = server.Databases[connectionDetails.Database];

                var model = new DatabaseModel();
                if (configuration.UserDefinedTableTypes != null)
                {
                    var userDefinedTypesExtractor = new UserDefinedTableTypeExtractor(database.UserDefinedTableTypes);
                    model.UserDefinedTableTypes = userDefinedTypesExtractor.Extract(configuration);
                }
                if (configuration.StoredProcedures != null)
                {
                    model.StoredProcedures = _storedProcedureExtractor.Extract(configuration, database.StoredProcedures);
                }

                return model;
            }
            finally
            {
                if (server.ConnectionContext.IsOpen)
                {
                    server.ConnectionContext.Disconnect();
                }
            }
        }

        private sealed class ConnectionDetails
        {
            public string DataSource { get; set; }
            public string Database { get; set; }
        }

        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IStoredProcedureExtractor _storedProcedureExtractor;
    }
}