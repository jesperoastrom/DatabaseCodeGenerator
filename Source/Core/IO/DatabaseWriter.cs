using System;
using System.IO;
using SqlFramework.Configuration;
using SqlFramework.Data.Extractors;
using SqlFramework.Data.Models;



namespace SqlFramework.IO
{

	public sealed class DatabaseWriter : IDatabaseWriter
	{

		public DatabaseWriter(
			IConfigurationReader configurationReader,
			IDatabaseExtractor databaseExtractor,
			IStorageProvider storageProvider,
			ITextWriter traceWriter)
		{
			this.configurationReader = configurationReader;
			this.databaseExtractor = databaseExtractor;
			this.storageProvider = storageProvider;
			this.traceOutput = traceWriter;
		}



		public bool WriteOutput(string configurationFile, string outputFile, string indentation)
		{
			DatabaseConfiguration configuration;

			if (this.configurationReader.TryRead(configurationFile, out configuration))
			{
				string directory = this.storageProvider.GetDirectoryName(outputFile);

				if (!storageProvider.DirectoryExists(directory))
				{
					throw new ArgumentException("Directory '" + directory + "' does not exist");
				}

				DatabaseModel databaseModel;
				if (TryGetDatabaseModel(configuration, out databaseModel))
				{
					WriteOutput(configuration, outputFile, indentation, databaseModel);
					return true;
				}

			}
			return false;
		}



		private bool TryGetDatabaseModel(DatabaseConfiguration configuration, out DatabaseModel databaseModel)
		{
			try
			{
				databaseModel = databaseExtractor.Extract(configuration);
				return true;
			}
			catch (Exception ex)
			{
				this.traceOutput.WriteLine(ex.Message);
				this.traceOutput.WriteLine(ex.StackTrace);
				databaseModel = null;
				return false;
			}
		}

		private void WriteOutput(DatabaseConfiguration configuration, string outputFile, string indentation, DatabaseModel databaseModel)
		{
			using (ICodeWriter writer = this.storageProvider.CreateOrOpenCodeWriter(outputFile, indentation))
			{
				if (databaseModel.StoredProcedures != null || databaseModel.UserDefinedTableTypes != null)
				{
					WriteUsings(writer);
				}

				if (databaseModel.StoredProcedures != null)
				{
					new StoredProcedureWriter(writer).Write(databaseModel.StoredProcedures);
				}

				if (databaseModel.UserDefinedTableTypes != null)
				{
					new UserDefinedTableTypeWriter(writer).Write(databaseModel.UserDefinedTableTypes);
				}
			}
		}

		private void WriteUsings(ICodeWriter writer)
		{
			writer
				.WriteIndentedLine("using System;")
				.WriteIndentedLine("using System.Collections.Generic;")
				.WriteIndentedLine("using System.Data;")
				.WriteIndentedLine("using System.Data.SqlClient;")
				.WriteNewLine();
		}



		private readonly IConfigurationReader configurationReader;
		private readonly IDatabaseExtractor databaseExtractor;
		private readonly IStorageProvider storageProvider;
		private readonly ITextWriter traceOutput;

	}



}
