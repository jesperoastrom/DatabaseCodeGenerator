using System;
using System.IO;
using Flip.Tools.Database.CodeGenerator.Configuration;
using Flip.Tools.Database.CodeGenerator.Data.Extractors;
using Flip.Tools.Database.CodeGenerator.Data.Models;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	public sealed class DatabaseWriter
	{

		public DatabaseWriter(TextWriter traceOutput)
		{
			this.traceOutput = traceOutput;
		}



		public bool WriteFile(string configurationFile, string outputFile, string connectionString, string indentation)
		{
			var configurationReader = new ConfigurationReader(configurationFile, this.traceOutput);

			DatabaseConfiguration configuration;
			if (configurationReader.TryRead(out configuration))
			{
				string directory = Path.GetDirectoryName(outputFile);

				if (!Directory.Exists(directory))
				{
					throw new ArgumentException("Directory '" + directory + "' does not exist");
				}

				DatabaseModel databaseModel;
				if (TryGetDatabaseModel(connectionString, configuration, out databaseModel))
				{
					WriteFile(configuration, outputFile, indentation, databaseModel);
					return true;
				}

			}
			return false;
		}



		private bool TryGetDatabaseModel(string connectionString, DatabaseConfiguration configuration, out DatabaseModel databaseModel)
		{
			try
			{
				var extractor = new DatabaseExtractor(connectionString);
				databaseModel = extractor.Extract(configuration);
				return true;
			}
			catch (Exception ex)
			{
				this.traceOutput.WriteLine(ex.Message);
				databaseModel = null;
				return false;
			}
		}

		private void WriteFile(DatabaseConfiguration configuration, string outputFile, string indentation, DatabaseModel databaseModel)
		{
			using (FileStream stream = File.Open(outputFile, FileMode.Create))
			{
				using (var writer = new Writer(stream, indentation))
				{
					if (databaseModel.StoredProcedures != null)
					{
						StoredProcedureWriter procedureWriter = new StoredProcedureWriter(writer);
						procedureWriter.Write(databaseModel.StoredProcedures);
					}
				}
			}
		}



		private readonly TextWriter traceOutput;

	}



}
