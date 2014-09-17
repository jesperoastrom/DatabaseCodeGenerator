namespace SqlFramework.Data.Extractors.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using Builders;
    using Configuration;
    using Microsoft.SqlServer.Management.Smo;
    using Models;

    public sealed class StoredProcedureExtractor
    {
        public StoredProcedureExtractor(
            IConnectionStringProvider connectionStringProvider,
            IDatabaseToCodeNameConverter nameConverter,
            ITypeConverter typeConverter,
            ITypeNameBuilder typeNameBuilder,
            ISchemaElementCollectionBuilder schemaElementCollectionBuilder)
        {
            _connectionStringProvider = connectionStringProvider;
            _nameConverter = nameConverter;
            _typeConverter = typeConverter;
            _typeNameBuilder = typeNameBuilder;
            _schemaElementCollectionBuilder = schemaElementCollectionBuilder;
        }

        public SchemaCollection<StoredProcedureModel> Extract(
            DatabaseConfiguration configuration,
            StoredProcedureCollection procedures)
        {
            Dictionary<string, StoredProcedure> storedProcedureLookup = CreateLookup(procedures);

            using (var connection = new SqlConnection(_connectionStringProvider.ConnectionString))
            {
                connection.Open();

                var collection = new SchemaCollection<StoredProcedureModel>(
                    _schemaElementCollectionBuilder,
                    configuration.StoredProcedures.Namespace);

                foreach (var element in configuration.StoredProcedures.Elements)
                {
                    string escapedFullName = _nameConverter.EscapeDatabaseName(element.SchemaName, element.Name);
                    if (storedProcedureLookup.ContainsKey(escapedFullName))
                    {
                        StoredProcedure procedure = storedProcedureLookup[escapedFullName];
                        StoredProcedureModel model = ToModel(connection, configuration, procedure);
                        collection.AddElement(procedure.Schema, model);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Unable to locate stored procedure '{0}'", escapedFullName));
                    }
                }

                return collection;
            }
        }

        private DatabaseName CreateDatabaseName(StoredProcedure procedure)
        {
            return new DatabaseName
                       {
                           SchemaName = procedure.Schema,
                           EscapedSchemaName = _nameConverter.EscapeDatabaseName(procedure.Name),
                           Name = procedure.Name,
                           EscapedName = _nameConverter.EscapeDatabaseName(procedure.Name),
                           EscapedFullName = _nameConverter.EscapeDatabaseName(procedure.Schema, procedure.Name)
                       };
        }

        private Dictionary<string, StoredProcedure> CreateLookup(StoredProcedureCollection procedures)
        {
            return procedures
                .Cast<StoredProcedure>()
                .ToDictionary(s => _nameConverter.EscapeDatabaseName(s.Schema, s.Name), StringComparer.OrdinalIgnoreCase);
        }

        private int? GetNumericPrecision(DataType dataType)
        {
            return
                dataType.SqlDataType == SqlDataType.Decimal ||
                dataType.SqlDataType == SqlDataType.Float ||
                dataType.SqlDataType == SqlDataType.Numeric ||
                dataType.SqlDataType == SqlDataType.Real
                    ? new int?(dataType.NumericPrecision)
                    : null;
        }

        private int? GetNumericScale(DataType dataType)
        {
            return
                dataType.SqlDataType == SqlDataType.Decimal ||
                dataType.SqlDataType == SqlDataType.Float ||
                dataType.SqlDataType == SqlDataType.Numeric ||
                dataType.SqlDataType == SqlDataType.Real
                    ? new int?(dataType.NumericScale)
                    : null;
        }

        private List<ParameterModel> GetParameters(DatabaseConfiguration configuration, StoredProcedure procedure)
        {
            return
                procedure.Parameters
                    .Cast<StoredProcedureParameter>()
                    .Select(p => ToModel(configuration, p))
                    .ToList();
        }

        private StoredProcedureResultModel GetResult(SqlDataReader reader)
        {
            DataTable table = reader.GetSchemaTable();

            if (table == null)
            {
                return null;
            }

            var model = new StoredProcedureResultModel();
            model.Columns = new List<ColumnModel>(table.Columns.Count);

            foreach (DataRow row in table.Rows)
            {
                var type = (Type)row["DataType"];
                var allowDbNull = (bool?)row["AllowDBNull"];
                model.Columns.Add(new ColumnModel()
                                      {
                                          DatabaseName = row["ColumnName"] as string,
                                          ClrType = _typeConverter.ToClrType(type, allowDbNull == true)
                                      });
            }

            return model;
        }

        private List<StoredProcedureResultModel> GetResults(SqlConnection connection, StoredProcedureModel model)
        {
            var list = new List<StoredProcedureResultModel>();

            using (var command = new SqlCommand())
            {
                var sb = new StringBuilder(128);
                sb.Append("set fmtonly on;exec ");
                sb.Append(model.DatabaseName.EscapedFullName);
                foreach (var parameter in model.Parameters)
                {
                    sb.Append(" ");
                    if (parameter.IncludeInFmtOnlyQuery())
                    {
                        sb.Append(parameter.Column.DatabaseName);
                        sb.Append("=null,");
                    }
                }
                if (model.Parameters.Count > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append(";");
                sb.Append("set fmtonly off;");
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = sb.ToString();

                using (var reader = command.ExecuteReader())
                {
                    //In case there is no result from the SP, the first result is null
                    StoredProcedureResultModel result = GetResult(reader);
                    if (result != null)
                    {
                        list.Add(result);
                    }

                    while (reader.NextResult())
                    {
                        list.Add(GetResult(reader));
                    }
                }
            }

            return list;
        }

        private int? GetSize(DataType dataType)
        {
            return
                dataType.SqlDataType == SqlDataType.Char ||
                dataType.SqlDataType == SqlDataType.NChar ||
                dataType.SqlDataType == SqlDataType.NText ||
                dataType.SqlDataType == SqlDataType.NVarChar ||
                dataType.SqlDataType == SqlDataType.NVarCharMax ||
                dataType.SqlDataType == SqlDataType.Text ||
                dataType.SqlDataType == SqlDataType.VarBinary ||
                dataType.SqlDataType == SqlDataType.VarChar ||
                dataType.SqlDataType == SqlDataType.VarBinaryMax ||
                dataType.SqlDataType == SqlDataType.VarCharMax
                    ? new int?(dataType.MaximumLength)
                    : null;
        }

        private StoredProcedureModel ToModel(SqlConnection connection, DatabaseConfiguration configuration, StoredProcedure procedure)
        {
            var model = new StoredProcedureModel();
            model.DatabaseName = CreateDatabaseName(procedure);
            model.TypeName = _typeNameBuilder.Build(configuration.StoredProcedures.Namespace, procedure.Name);
            model.Parameters = GetParameters(configuration, procedure);
            model.OutputParameters = model.Parameters.Where(p => p.IsOutput).ToList();
            model.Results = GetResults(connection, model);
            return model;
        }

        private ParameterModel ToModel(DatabaseConfiguration configuration, StoredProcedureParameter p)
        {
            return new ParameterModel
                       {
                           Scale = GetNumericScale(p.DataType),
                           Precision = GetNumericPrecision(p.DataType),
                           Size = GetSize(p.DataType),
                           IsOutput = p.IsOutputParameter,
                           //todo
                           //SqlDataType = p.DataType.SqlDataType,
                           SqlDbType = _typeConverter.ToSqlDbDataType(p.DataType),
                           Column = new ColumnModel()
                                        {
                                            DatabaseName = p.Name,
                                            ClrType = _typeConverter.ToClrType(p, configuration.TableTypeNamespaceFromStoredProcedure)
                                        }
                       };
        }

        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IDatabaseToCodeNameConverter _nameConverter;
        private readonly ISchemaElementCollectionBuilder _schemaElementCollectionBuilder;
        private readonly ITypeConverter _typeConverter;
        private readonly ITypeNameBuilder _typeNameBuilder;
    }
}