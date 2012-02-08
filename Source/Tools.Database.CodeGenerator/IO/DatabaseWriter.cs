using System;
using System.IO;
using Flip.Tools.Database.CodeGenerator.Configuration;
using Flip.Tools.Database.CodeGenerator.Data.Extractors;
using Flip.Tools.Database.CodeGenerator.Data.Models;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	public sealed class DatabaseWriter : IDatabaseWriter
	{

		public DatabaseWriter(
			IConfigurationReader configurationReader,
			IDatabaseExtractor databaseExtractor,
			IStorageProvider outputFacade,
			ITextWriter traceWriter)
		{
			this.configurationReader = configurationReader;
			this.databaseExtractor = databaseExtractor;
			this.outputFacade = outputFacade;
			this.traceOutput = traceWriter;
		}



		public bool WriteOutput(string configurationFile, string outputFile, string indentation)
		{
			DatabaseConfiguration configuration;

			if (this.configurationReader.TryRead(configurationFile, out configuration))
			{
				string directory = Path.GetDirectoryName(outputFile);

				if (!outputFacade.OutputDirectoryExists(directory))
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
			using (ICodeWriter writer = this.outputFacade.CreateOrOpenOutputWriter(outputFile, indentation))
			{
				if (databaseModel.StoredProcedures != null)
				{
					new StoredProcedureWriter(writer).Write(databaseModel.StoredProcedures);
				}
			}
		}



		private readonly IConfigurationReader configurationReader;
		private readonly IDatabaseExtractor databaseExtractor;
		private readonly IStorageProvider outputFacade;
		private readonly ITextWriter traceOutput;

	}



}
