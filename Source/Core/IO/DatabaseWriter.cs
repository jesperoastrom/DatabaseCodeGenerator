using System;
using SqlFramework.Configuration;
using SqlFramework.Data;
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
            _configurationReader = configurationReader;
            _databaseExtractor = databaseExtractor;
            _storageProvider = storageProvider;
            _traceOutput = traceWriter;
        }

        private bool TryGetConfiguration(string configurationFile, out DatabaseConfiguration configuration)
        {
            try
            {
                configuration = _configurationReader.Read(configurationFile);
                return true;
            }
            catch (Exception ex)
            {
                _traceOutput.WriteLine(ex.Message);
                _traceOutput.WriteLine(ex.StackTrace);
                configuration = null;
                return false;
            }
        }

        private bool TryGetDatabaseModel(DatabaseConfiguration configuration, out DatabaseModel databaseModel)
        {
            try
            {
                databaseModel = _databaseExtractor.Extract(configuration);
                return true;
            }
            catch (Exception ex)
            {
                _traceOutput.WriteLine(ex.Message);
                _traceOutput.WriteLine(ex.StackTrace);
                databaseModel = null;
                return false;
            }
        }

        private void WriteOutput(string outputFile, string indentation, DatabaseModel databaseModel)
        {
            using (ICodeWriter writer = _storageProvider.CreateOrOpenCodeWriter(outputFile, indentation))
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

        public bool WriteOutput(string configurationFile, string outputFile, string indentation)
        {
            string directory = _storageProvider.GetDirectoryName(outputFile);

            if (!_storageProvider.DirectoryExists(directory))
            {
                throw new ArgumentException("Directory '" + directory + "' does not exist");
            }

            DatabaseConfiguration configuration;
            if (!TryGetConfiguration(configurationFile, out configuration))
            {
                return false;
            }

            DatabaseModel databaseModel;
            if (TryGetDatabaseModel(configuration, out databaseModel))
            {
                WriteOutput(outputFile, indentation, databaseModel);
                return true;
            }
            return false;
        }

        private readonly IConfigurationReader _configurationReader;
        private readonly IDatabaseExtractor _databaseExtractor;
        private readonly IStorageProvider _storageProvider;
        private readonly ITextWriter _traceOutput;
    }
}