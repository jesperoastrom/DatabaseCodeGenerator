using System.Data.SqlClient;
using Flip.Tools.Database.CodeGenerator.Data.Models;
using Microsoft.SqlServer.Management.Common;
using Smo = Microsoft.SqlServer.Management.Smo;



namespace Flip.Tools.Database.CodeGenerator.Data.Extractors
{

	internal sealed class DatabaseExtractor
	{

		public DatabaseExtractor(string connectionString)
		{
			this.connectionString = connectionString;
		}



		public DatabaseModel Extract(Configuration.DatabaseConfiguration configuration)
		{
			ConnectionDetails connectionDetails = GetConnectionDetails();

			var serverConnection = new ServerConnection(connectionDetails.DataSource);
			var server = new Smo.Server(serverConnection);

			try
			{
				server.ConnectionContext.Connect();

				Smo.Database database = server.Databases[connectionDetails.Database];

				var model = new DatabaseModel();
				if (configuration.UserDefinedTableTypes != null)
				{
					var userDefinedTypesExtractor = new UserDefinedTableTypeExtractor(database.UserDefinedTableTypes);
					model.UserDefinedTableTypes = userDefinedTypesExtractor.Extract(configuration.UserDefinedTableTypes);
				}
				if (configuration.StoredProcedures != null)
				{
					var storedProceduresExtractor = new StoredProcedureExtractor(this.connectionString, database.StoredProcedures);
					model.StoredProcedures = storedProceduresExtractor.Extract(configuration.StoredProcedures);
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


		private ConnectionDetails GetConnectionDetails()
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(this.connectionString))
				{
					connection.Open();
					return new ConnectionDetails()
					{
						DataSource = connection.DataSource,
						Database = connection.Database
					};
				}
			}
			catch (SqlException sqlException)
			{
				throw new InvalidArgumentException("Could not connect to database using connection string '" + this.connectionString + "'.", sqlException);
			}
		}



		private readonly string connectionString;



		private sealed class ConnectionDetails
		{

			public string DataSource { get; set; }
			public string Database { get; set; }

		}

	}

}
