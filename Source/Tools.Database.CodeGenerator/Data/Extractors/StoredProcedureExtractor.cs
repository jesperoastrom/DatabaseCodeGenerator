using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Flip.Tools.Database.CodeGenerator.Data.Models;
using Smo = Microsoft.SqlServer.Management.Smo;



namespace Flip.Tools.Database.CodeGenerator.Data.Extractors
{

	internal sealed class StoredProcedureExtractor
	{

		public SchemaCollection<StoredProcedureModel> Extract(
			IConnectionStringProvider connectionStringProvider,
			Configuration.DatabaseConfiguration configuration,
			Smo.StoredProcedureCollection procedures)
		{
			Dictionary<string, Smo.StoredProcedure> storedProcedureLookup = procedures
				.Cast<Smo.StoredProcedure>()
				.ToDictionary(t => t.Schema.EscapeDatabaseName() + "." + t.Name.EscapeDatabaseName(), StringComparer.OrdinalIgnoreCase);

			using (SqlConnection connection = new SqlConnection(connectionStringProvider.ConnectionString))
			{
				connection.Open();

				var collection = new SchemaCollection<StoredProcedureModel>(configuration.StoredProcedures.Namespace);

				foreach (var element in configuration.StoredProcedures.Elements)
				{
					if (storedProcedureLookup.ContainsKey(element.EscapedFullName))
					{
						Smo.StoredProcedure procedure = storedProcedureLookup[element.EscapedFullName];
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



		private StoredProcedureModel ToModel(SqlConnection connection, Configuration.DatabaseConfiguration configuration, Smo.StoredProcedure procedure)
		{
			var model = new StoredProcedureModel();
			model.DatabaseName = new DatabaseName(procedure.Schema, procedure.Name);
			model.TypeName = new TypeName(configuration.StoredProcedures.Namespace, procedure.Name);
			model.Parameters = GetParameters(configuration, procedure);
			model.OutputParameters = model.Parameters.Where(p => p.IsOutput).ToList();
			model.Results = GetResults(connection, model);
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
				Type type = (Type)row["DataType"];
				bool? allowDbNull = (bool?)row["AllowDBNull"];
				model.Columns.Add(new ColumnModel()
				{
					DatabaseName = row["ColumnName"] as string,
					ClrType = type.ToClrString(allowDbNull == true)
				});
			}

			return model;
		}

		private List<ParameterModel> GetParameters(Configuration.DatabaseConfiguration configuration, Smo.StoredProcedure procedure)
		{
			return
				procedure.Parameters
				.Cast<Smo.StoredProcedureParameter>()
				.Select(p => ToModel(configuration, p))
				.ToList();
		}

		private ParameterModel ToModel(Configuration.DatabaseConfiguration configuration, Smo.StoredProcedureParameter p)
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
					ClrType = p.ToClrString(configuration.TableTypeNamespaceFromStoredProcedure)
				}
			};
		}

		private int? GetNumericScale(Smo.DataType dataType)
		{
			return
				dataType.SqlDataType == Smo.SqlDataType.Decimal ||
				dataType.SqlDataType == Smo.SqlDataType.Float ||
				dataType.SqlDataType == Smo.SqlDataType.Numeric ||
				dataType.SqlDataType == Smo.SqlDataType.Real ?
				new int?(dataType.NumericScale) :
				null;
		}

		private int? GetNumericPrecision(Smo.DataType dataType)
		{
			return
				dataType.SqlDataType == Smo.SqlDataType.Decimal ||
				dataType.SqlDataType == Smo.SqlDataType.Float ||
				dataType.SqlDataType == Smo.SqlDataType.Numeric ||
				dataType.SqlDataType == Smo.SqlDataType.Real ?
				new int?(dataType.NumericPrecision) :
				null;
		}

		private int? GetSize(Smo.DataType dataType)
		{
			return
				dataType.SqlDataType == Smo.SqlDataType.Char ||
				dataType.SqlDataType == Smo.SqlDataType.NChar ||
				dataType.SqlDataType == Smo.SqlDataType.NText ||
				dataType.SqlDataType == Smo.SqlDataType.NVarChar ||
				dataType.SqlDataType == Smo.SqlDataType.NVarCharMax ||
				dataType.SqlDataType == Smo.SqlDataType.Text ||
				dataType.SqlDataType == Smo.SqlDataType.VarBinary ||
				dataType.SqlDataType == Smo.SqlDataType.VarChar ||
				dataType.SqlDataType == Smo.SqlDataType.VarBinaryMax ||
				dataType.SqlDataType == Smo.SqlDataType.VarCharMax ?
				new int?(dataType.MaximumLength) :
				null;
		}

	}

}
