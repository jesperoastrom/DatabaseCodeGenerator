using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SqlFramework.Data.Models;

namespace SqlFramework.Data.Extractors.SqlServer2012
{
    internal sealed class StoredProcedureExtractor
    {
        public SchemaCollection<StoredProcedureModel> Extract(
            IConnectionStringProvider connectionStringProvider,
            Configuration.DatabaseConfiguration configuration,
            Microsoft.SqlServer.Management.Smo.StoredProcedureCollection procedures)
        {
            Dictionary<string, Microsoft.SqlServer.Management.Smo.StoredProcedure> storedProcedureLookup = procedures
                .Cast<Microsoft.SqlServer.Management.Smo.StoredProcedure>()
                .ToDictionary(t => t.Schema.EscapeDatabaseName() + "." + t.Name.EscapeDatabaseName(), StringComparer.OrdinalIgnoreCase);

            using (SqlConnection connection = new SqlConnection(connectionStringProvider.ConnectionString))
            {
                connection.Open();

                var collection = new SchemaCollection<StoredProcedureModel>(configuration.StoredProcedures.Namespace);

                foreach (var element in configuration.StoredProcedures.Elements)
                {
                    if (storedProcedureLookup.ContainsKey(element.EscapedFullName))
                    {
                        Microsoft.SqlServer.Management.Smo.StoredProcedure procedure = storedProcedureLookup[element.EscapedFullName];
                        StoredProcedureModel model = ToModel(connection, configuration, procedure);
                        collection.AddElement(procedure.Schema, model);
                    }
                    else
                    {
                        throw new ArgumentException("Unable to locate stored procedure '" + element.EscapedFullName + "'");
                    }
                }

                return collection;
            }
        }

        private int? GetNumericPrecision(Microsoft.SqlServer.Management.Smo.DataType dataType)
        {
            return
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.Decimal ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.Float ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.Numeric ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.Real ?
                    new int?(dataType.NumericPrecision) :
                    null;
        }

        private int? GetNumericScale(Microsoft.SqlServer.Management.Smo.DataType dataType)
        {
            return
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.Decimal ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.Float ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.Numeric ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.Real ?
                    new int?(dataType.NumericScale) :
                    null;
        }

        private List<ParameterModel> GetParameters(Configuration.DatabaseConfiguration configuration, Microsoft.SqlServer.Management.Smo.StoredProcedure procedure)
        {
            return
                procedure.Parameters
                         .Cast<Microsoft.SqlServer.Management.Smo.StoredProcedureParameter>()
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

            StoredProcedureResultModel model = new StoredProcedureResultModel();
            model.Columns = new List<ColumnModel>(table.Columns.Count);

            foreach (DataRow row in table.Rows)
            {
                Type type = (Type) row["DataType"];
                bool? allowDbNull = (bool?) row["AllowDBNull"];
                model.Columns.Add(new ColumnModel()
                                      {
                                          DatabaseName = row["ColumnName"] as string,
                                          ClrType = type.ToClrType(allowDbNull == true)
                                      });
            }

            return model;
        }

        private List<StoredProcedureResultModel> GetResults(SqlConnection connection, StoredProcedureModel model)
        {
            List<StoredProcedureResultModel> list = new List<StoredProcedureResultModel>();

            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder sb = new StringBuilder(128);
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

        private int? GetSize(Microsoft.SqlServer.Management.Smo.DataType dataType)
        {
            return
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.Char ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.NChar ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.NText ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.NVarChar ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.NVarCharMax ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.Text ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.VarBinary ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.VarChar ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.VarBinaryMax ||
                dataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.VarCharMax ?
                    new int?(dataType.MaximumLength) :
                    null;
        }

        private StoredProcedureModel ToModel(SqlConnection connection, Configuration.DatabaseConfiguration configuration, Microsoft.SqlServer.Management.Smo.StoredProcedure procedure)
        {
            var model = new StoredProcedureModel();
            model.DatabaseName = new DatabaseName(procedure.Schema, procedure.Name);
            model.TypeName = new TypeName(configuration.StoredProcedures.Namespace, procedure.Name);
            model.Parameters = GetParameters(configuration, procedure);
            model.OutputParameters = model.Parameters.Where(p => p.IsOutput).ToList();
            model.Results = GetResults(connection, model);
            return model;
        }

        private ParameterModel ToModel(Configuration.DatabaseConfiguration configuration, Microsoft.SqlServer.Management.Smo.StoredProcedureParameter p)
        {
            return new ParameterModel()
                       {
                           Scale = GetNumericScale(p.DataType),
                           Precision = GetNumericPrecision(p.DataType),
                           Size = GetSize(p.DataType),
                           IsOutput = p.IsOutputParameter,
                           SqlDataType = p.DataType.SqlDataType,
                           SqlDbType = p.DataType.ToSqlDbDataType(),
                           Column = new ColumnModel()
                                        {
                                            DatabaseName = p.Name,
                                            ClrType = p.ToClrType(configuration.TableTypeNamespaceFromStoredProcedure)
                                        }
                       };
        }
    }
}