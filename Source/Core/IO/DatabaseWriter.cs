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
            ITextWriter traceWriter,
            IWriterFactory writerFactory)
        {
            _configurationReader = configurationReader;
            _databaseExtractor = databaseExtractor;
            _storageProvider = storageProvider;
            _traceOutput = traceWriter;
            _writerFactory = writerFactory;
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
                    _writerFactory
                        .CreateUsingsWriter(writer)
                        .WriteUsings();
                }

                if (databaseModel.StoredProcedures != null)
                {
                    _writerFactory
                        .CreateStoredProcedureWriter(writer)
                        .Write(databaseModel.StoredProcedures);
                }

                if (databaseModel.UserDefinedTableTypes != null)
                {
                    _writerFactory
                        .CreateUserDefinedTableTypeWriter(writer)
                        .Write(databaseModel.UserDefinedTableTypes);
                }
            }
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
        private readonly IWriterFactory _writerFactory;
    }
}