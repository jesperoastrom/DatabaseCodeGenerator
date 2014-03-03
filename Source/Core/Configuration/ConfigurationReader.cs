using System;
using SqlFramework.IO;

namespace SqlFramework.Configuration
{
    public sealed class ConfigurationReader : IConfigurationReader
    {
        public ConfigurationReader(ITextWriter errorOutput, IStorageProvider storageProvider)
        {
            _errorOutput = errorOutput;
            _storageProvider = storageProvider;
        }

        private void WriteException(Exception ex)
        {
            _errorOutput.WriteLine(ex.Message);
            _errorOutput.WriteLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                WriteException(ex.InnerException);
            }
        }

        public bool TryRead(string file, out DatabaseConfiguration configuration)
        {
            if (!_storageProvider.FileExists(file))
            {
                _errorOutput.WriteLine("Unable to find file '" + file + "'");
                configuration = null;
                return false;
            }

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof (DatabaseConfiguration));

            try
            {
                using (var stream = this._storageProvider.OpenStream(file))
                {
                    configuration = (DatabaseConfiguration) serializer.Deserialize(stream);
                    configuration.TableTypeNamespaceFromStoredProcedure =
                        configuration.StoredProcedures.Namespace.GetShortestNamespace(configuration.UserDefinedTableTypes.Namespace);
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                configuration = null;
                return false;
            }
        }

        private readonly ITextWriter _errorOutput;
        private readonly IStorageProvider _storageProvider;
    }
}