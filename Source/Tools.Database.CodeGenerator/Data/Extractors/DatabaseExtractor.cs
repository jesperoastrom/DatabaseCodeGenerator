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
			Smo.Database database = CreateDatabase();

			var userDefinedTypesExtractor = new UserDefinedTableTypeExtractor(database.UserDefinedTableTypes);
			var storedProceduresExtractor = new StoredProcedureExtractor(database.StoredProcedures);

			var model = new DatabaseModel();
			model.UserDefinedTableTypes = userDefinedTypesExtractor.Extract(configuration.UserDefinedTableTypes.Elements);
			model.StoredProcedures = storedProceduresExtractor.Extract(configuration.StoredProcedures.Elements);
			return model;
		}



		private Smo.Database CreateDatabase()
		{
			string dataSource = null;

			try
			{
				using (SqlConnection connection = new SqlConnection(this.connectionString))
				{
					dataSource = connection.DataSource;
					connection.Open();
				}
			}
			catch (SqlException sqlException)
			{
				throw new InvalidArgumentException("Could not connect to database using connection string '" + this.connectionString + "'.", sqlException);
			}

			var serverConnection = new ServerConnection(this.connectionString);
			var server = new Smo.Server(serverConnection);
			var database = new Smo.Database(server, dataSource);
			return database;
		}



		private readonly string connectionString;

	}

}
