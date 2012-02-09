﻿using System;
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
			model.Parameters = ToModel(configuration, procedure.Parameters);
			model.Results = GetResults(connection, model);
			return model;
		}

		private List<ResultModel> GetResults(SqlConnection connection, StoredProcedureModel model)
		{
			List<ResultModel> list = new List<ResultModel>();

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
					list.Add(GetResult(reader));

					while (reader.NextResult())
					{
						list.Add(GetResult(reader));
					}
				}
			}

			return list;
		}

		private ResultModel GetResult(SqlDataReader reader)
		{
			DataTable table = reader.GetSchemaTable();

			ResultModel model = new ResultModel();
			model.Columns = new List<ColumnModel>(table.Columns.Count);

			foreach(DataRow row in table.Rows)
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

		private List<ParameterModel> ToModel(Configuration.DatabaseConfiguration configuration, Smo.StoredProcedureParameterCollection parameters)
		{
			return parameters.Cast<Smo.StoredProcedureParameter>().Select(p => new ParameterModel()
			{
				IsOutput = p.IsOutputParameter,
				SqlDataType = p.DataType.SqlDataType,
				SqlDbType = p.DataType.ToSqlDbDataType(),
				Column = new ColumnModel()
				{
					DatabaseName = p.Name,
					ClrType = p.ToClrString(configuration.TableTypeNamespaceFromStoredProcedure)
				}
			}).ToList();
		}

	}

}
