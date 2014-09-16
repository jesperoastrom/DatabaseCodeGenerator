using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;
using SqlFramework.Data.Builders;
using SqlFramework.Data.Models;

namespace SqlFramework.Data.Extractors.SqlServer
{
    public sealed class UserDefinedTableTypeExtractor
    {
        public UserDefinedTableTypeExtractor(
            IDatabaseToCodeNameConverter nameConverter,
            ITypeConverter typeConverter,
            ITypeNameBuilder typeNameBuilder,
            IDatabaseNameBuilder databaseNameBuilder,
            ISchemaElementCollectionBuilder schemaElementCollectionBuilder)
        {
            _nameConverter = nameConverter;
            _typeConverter = typeConverter;
            _typeNameBuilder = typeNameBuilder;
            _databaseNameBuilder = databaseNameBuilder;
            _schemaElementCollectionBuilder = schemaElementCollectionBuilder;
        }

        public UserDefinedTableTypeExtractor(UserDefinedTableTypeCollection tableTypes)
        {
            _tableTypeLookup = tableTypes
                .Cast<UserDefinedTableType>()
                .ToDictionary(t => _nameConverter.EscapeDatabaseName(t.Schema, t.Name), StringComparer.OrdinalIgnoreCase);
        }

        public SchemaCollection<UserDefinedTableTypeModel> Extract(Configuration.DatabaseConfiguration configuration)
        {
            var collection = new SchemaCollection<UserDefinedTableTypeModel>(
                _schemaElementCollectionBuilder,
                configuration.UserDefinedTableTypes.Namespace);

            foreach (var element in configuration.UserDefinedTableTypes.Elements)
            {
                string escapedFullName = _nameConverter.EscapeDatabaseName(element.SchemaName, element.Name);
                if (_tableTypeLookup.ContainsKey(escapedFullName))
                {
                    UserDefinedTableType tableType = _tableTypeLookup[escapedFullName];
                    collection.AddElement(tableType.Schema, ToModel(configuration.UserDefinedTableTypes.Namespace, tableType));
                }
                else
                {
                    throw new ArgumentException("Unable to locate user defined type '" + escapedFullName + "'");
                }
            }

            return collection;
        }

        private UserDefinedTableTypeModel ToModel(string typeNamespace, UserDefinedTableType tableType)
        {
            return new UserDefinedTableTypeModel
                       {
                           DatabaseName = _databaseNameBuilder.Build(tableType.Schema, tableType.Name),
                           TypeName = _typeNameBuilder.Build(typeNamespace, tableType.Name),
                           Columns = ToModel(typeNamespace, tableType.Columns)
                       };
        }

        private List<ColumnModel> ToModel(string typeNamespace, ColumnCollection columns)
        {
            return columns.Cast<Column>().Select(c => new ColumnModel
                                                          {
                                                              DatabaseName = c.Name,
                                                              ClrType = _typeConverter.ToClrType(c, typeNamespace)
                                                          }).ToList();
        }

        private readonly IDatabaseNameBuilder _databaseNameBuilder;
        private readonly IDatabaseToCodeNameConverter _nameConverter;
        private readonly ITypeConverter _typeConverter;
        private readonly ISchemaElementCollectionBuilder _schemaElementCollectionBuilder;
        private readonly Dictionary<string, UserDefinedTableType> _tableTypeLookup;
        private readonly ITypeNameBuilder _typeNameBuilder;
    }
}