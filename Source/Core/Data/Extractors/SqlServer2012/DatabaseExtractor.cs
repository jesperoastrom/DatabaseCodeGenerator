using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SqlFramework.Configuration;
using SqlFramework.Data.Models;

namespace SqlFramework.Data.Extractors.SqlServer2012
{
    public sealed class DatabaseExtractor : IDatabaseExtractor
    {
        public DatabaseExtractor(IConnectionStringProvider connectionStringProvider)
        {
            this._connectionStringProvider = connectionStringProvider;
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
                throw new InvalidArgumentException("Could not connect to database using connection string '" + _connectionStringProvider.ConnectionString + "'.", sqlException);
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
                    model.StoredProcedures = new StoredProcedureExtractor().Extract(_connectionStringProvider, configuration, database.StoredProcedures);
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
    }
}