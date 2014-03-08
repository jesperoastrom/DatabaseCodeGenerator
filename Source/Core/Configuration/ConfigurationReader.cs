﻿using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SqlFramework.IO;

namespace SqlFramework.Configuration
{
    public sealed class ConfigurationReader : IConfigurationReader
    {
        public ConfigurationReader(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        private XmlReaderSettings CreateSettings(XmlSchemaSet schemas)
        {
            return new XmlReaderSettings
                       {
                           Schemas = schemas,
                           ValidationType = ValidationType.Schema,
                           ValidationFlags =
                               XmlSchemaValidationFlags.ProcessIdentityConstraints |
                               XmlSchemaValidationFlags.ReportValidationWarnings
                       };
        }

        private XmlSchemaSet GetSchemas()
        {
            using (var xsdStream = EmbeddedResourceHelper.GetStreamFromEmbeddedResource<Resources>(Resources.Configuration.DatabaseConfiguration_xsd))
            {
                var schemas = new XmlSchemaSet();
                schemas.Add(XmlSchema.Read(xsdStream, (sender, args) => { }));
                return schemas;
            }
        }

        public DatabaseConfiguration Read(string file)
        {
            if (!_storageProvider.FileExists(file))
            {
                throw new FileNotFoundException(file);
            }

            Exception firstException = null;
            XmlSchemaSet schemas = GetSchemas();
            XmlReaderSettings settings = CreateSettings(schemas);
            settings.ValidationEventHandler += (sender, args) =>
                                                   {
                                                       if (args.Severity == XmlSeverityType.Error && firstException == null)
                                                       {
                                                           firstException = args.Exception;
                                                       }
                                                   };
            DatabaseConfiguration configuration;
            using (var stream = _storageProvider.OpenStream(file))
            {
                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    var serializer = new XmlSerializer(typeof (DatabaseConfiguration));
                    configuration = (DatabaseConfiguration) serializer.Deserialize(reader);
                    configuration.TableTypeNamespaceFromStoredProcedure =
                        configuration.StoredProcedures.Namespace.GetShortestNamespace(configuration.UserDefinedTableTypes.Namespace);
                }
            }
            if (firstException != null)
            {
                throw new XmlSchemaException("Configuration file does not confirm to schema definition", firstException);
            }
            return configuration;
        }

        private readonly IStorageProvider _storageProvider;
    }
}