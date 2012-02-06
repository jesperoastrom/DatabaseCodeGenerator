using System;
using System.Collections.Generic;
using System.Linq;
using Flip.Tools.Database.CodeGenerator.Data.Models;
using Smo = Microsoft.SqlServer.Management.Smo;
using System.Data;
using System.Data.SqlClient;
using System.Text;



namespace Flip.Tools.Database.CodeGenerator.Data.Extractors
{

	internal sealed class StoredProcedureExtractor
	{

		public StoredProcedureExtractor(string connectionString, Smo.StoredProcedureCollection procedures)
		{
			this.connectionString = connectionString;
			this.storedProcedureLookup = procedures
				.Cast<Smo.StoredProcedure>()
				.ToDictionary(t => t.Schema.EscapeDatabaseName() + "." + t.Name.EscapeDatabaseName(), StringComparer.OrdinalIgnoreCase);
		}



		public SchemaCollection<StoredProcedureModel> Extract(Configuration.StoredProcedures storedProcedures)
		{
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				connection.Open();

				var collection = new SchemaCollection<StoredProcedureModel>(storedProcedures.Namespace);

				foreach (var element in storedProcedures.Elements)
				{
					if (this.storedProcedureLookup.ContainsKey(element.EscapedFullName))
					{
						Smo.StoredProcedure procedure = this.storedProcedureLookup[element.EscapedFullName];
						collection.AddElement(procedure.Schema, ToModel(connection, storedProcedures.Namespace, procedure));
					}
					else
					{
						throw new ArgumentException("Unable to locate stored procedure '" + element.EscapedFullName + "'");
					}
				}

				return collection;
			}
		}



		private StoredProcedureModel ToModel(SqlConnection connection, string procedureNamespace, Smo.StoredProcedure procedure)
		{
			var model = new StoredProcedureModel();
			model.DatabaseName = new DatabaseName(procedure.Schema, procedure.Name);
			model.TypeName = new TypeName(procedureNamespace, procedure.Name);
			model.Parameters = ToModel(procedure.Parameters);
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
					sb.Append(parameter.Column.DatabaseName);
					sb.Append("=null,");
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
					if (reader.Read())
					{
						list.Add(GetResult(reader));
						while (reader.NextResult())
						{
							if (reader.Read())
							{
								list.Add(GetResult(reader));
							}
						}
					}
				}
			}

			return list;
		}

		private ResultModel GetResult(SqlDataReader reader)
		{
			ResultModel model = new ResultModel();
			
			for(int i = 0; i< reader.FieldCount; i++)
			{
				model.Columns.Add(new ColumnModel()
				{
					DatabaseName = reader.GetName(i),
					ClrType = reader.GetFieldType(i).ToClrString()
				});
			}

			return model;
		}

		private List<ParameterModel> ToModel(Smo.StoredProcedureParameterCollection parameters)
		{
			return parameters.Cast<Smo.StoredProcedureParameter>().Select(p => new ParameterModel()
			{
				IsOutput = p.IsOutputParameter,
				SqlDbType = ToSqlDbType(p.DataType.SqlDataType),
				Column = new ColumnModel()
				{
					DatabaseName = p.Name,					
					ClrType = p.DataType.ToClrString()
				}
			}).ToList();
		}

		private SqlDbType ToSqlDbType(Smo.SqlDataType sqlDataType)
		{
			SqlDbType sqlDbType;
			switch (sqlDataType)
			{
				case Smo.SqlDataType.UserDefinedType:
					sqlDbType = System.Data.SqlDbType.Udt;
					break;
				//TODO
				//case Smo.SqlDataType.None:
				//case Smo.SqlDataType.NVarCharMax:
				//case Smo.SqlDataType.UserDefinedDataType:
				//case Smo.SqlDataType.VarBinaryMax:
				//case Smo.SqlDataType.VarCharMax:
				//case Smo.SqlDataType.SysName:
				//case Smo.SqlDataType.Numeric:
				//case Smo.SqlDataType.UserDefinedTableType:
				//case Smo.SqlDataType.HierarchyId:
				//case Smo.SqlDataType.Geometry:
				case Smo.SqlDataType.Geography:
					throw new NotSupportedException("Unable to convert to SqlDbType:" + sqlDataType);
				default:
					if (!Enum.TryParse<SqlDbType>(sqlDataType.ToString(), out sqlDbType))
					{
						throw new NotSupportedException("Unable to convert to SqlDbType:" + sqlDataType);
					}
					break;
			}
			return sqlDbType;
		}



		private readonly string connectionString;
		private readonly Dictionary<string, Smo.StoredProcedure> storedProcedureLookup;

	}

}
