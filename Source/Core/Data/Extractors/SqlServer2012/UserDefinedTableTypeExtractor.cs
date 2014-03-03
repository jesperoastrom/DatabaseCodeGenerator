using System;
using System.Collections.Generic;
using System.Linq;
using SqlFramework.Data.Models;

namespace SqlFramework.Data.Extractors.SqlServer2012
{
    internal sealed class UserDefinedTableTypeExtractor
    {
        public UserDefinedTableTypeExtractor(Microsoft.SqlServer.Management.Smo.UserDefinedTableTypeCollection tableTypes)
        {
            this.tableTypeLookup = tableTypes
                .Cast<Microsoft.SqlServer.Management.Smo.UserDefinedTableType>()
                .ToDictionary(t => t.Schema.EscapeDatabaseName() + "." + t.Name.EscapeDatabaseName(), StringComparer.OrdinalIgnoreCase);
        }

        public SchemaCollection<UserDefinedTableTypeModel> Extract(Configuration.DatabaseConfiguration configuration)
        {
            var collection = new SchemaCollection<UserDefinedTableTypeModel>(configuration.UserDefinedTableTypes.Namespace);

            foreach (var element in configuration.UserDefinedTableTypes.Elements)
            {
                if (this.tableTypeLookup.ContainsKey(element.EscapedFullName))
                {
                    Microsoft.SqlServer.Management.Smo.UserDefinedTableType tableType = this.tableTypeLookup[element.EscapedFullName];
                    collection.AddElement(tableType.Schema, ToModel(configuration.UserDefinedTableTypes.Namespace, tableType));
                }
                else
                {
                    throw new ArgumentException("Unable to locate user defined type '" + element.EscapedFullName + "'");
                }
            }

            return collection;
        }

        private UserDefinedTableTypeModel ToModel(string typeNamespace, Microsoft.SqlServer.Management.Smo.UserDefinedTableType tableType)
        {
            var model = new UserDefinedTableTypeModel();
            model.DatabaseName = new DatabaseName(tableType.Schema, tableType.Name);
            model.TypeName = new TypeName(typeNamespace, tableType.Name);
            model.Columns = ToModel(typeNamespace, tableType.Columns);
            return model;
        }

        private List<ColumnModel> ToModel(string typeNamespace, Microsoft.SqlServer.Management.Smo.ColumnCollection columns)
        {
            return columns.Cast<Microsoft.SqlServer.Management.Smo.Column>().Select(c => new ColumnModel()
                                                                                             {
                                                                                                 DatabaseName = c.Name,
                                                                                                 ClrType = c.ToClrType(typeNamespace)
                                                                                             }).ToList();
        }

        private readonly Dictionary<string, Microsoft.SqlServer.Management.Smo.UserDefinedTableType> tableTypeLookup;
    }
}