using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Flip.Tools.Database.CodeGenerator.Data.Models;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	public sealed class StoredProcedureWriter : ElementWriterBase
	{

		public StoredProcedureWriter(ICodeWriter writer)
			: base(writer)
		{
		}



		public void Write(SchemaCollection<StoredProcedureModel> storedProcedures)
		{
			WriteNamespaceStart(storedProcedures.ElementNamespace);

			foreach (var schema in storedProcedures.SchemaElementCollections.OrderBy(s => s.SchemaName))
			{
				BeginWriteStaticClass(schema.SchemaName);
				{
					List<StoredProcedureModel> elements = schema.Elements.OrderBy(s => s.DatabaseName).ToList();
					int lastIndex = elements.Count - 1;

					for (int i = 0; i < elements.Count; i++)
					{
						var element = elements[i];
						WriteProcedure(element, i == lastIndex);
					}

					WriteGetValueOrDefaultMethod();
				}
				WriteBlockEnd();
			}

			WriteBlockEnd();
		}



		private void WriteProcedure(StoredProcedureModel procedure, bool isLast)
		{
			this.writer
				.WriteIndentation()
				.Write("public partial class ")
				.Write(procedure.TypeName.Name)
				.WriteNewLine()
				.WriteIndentedLine("{");


			this.writer.Indent++;
			{
				if (procedure.Results.Count > 0)
				{
					WriteExecuteMethodOverload("ExecuteResult", procedure);
					WriteExecuteResultMethod(procedure);
				}
				else
				{
					WriteExecuteMethodOverload("ExecuteNonQuery", procedure);
					WriteExecuteNonQueryMethod(procedure);
				}

				WriteParameterClass(procedure);

				this.writer.WriteNewLine();
				WriteResultClass(procedure);
			}
			WriteBlockEnd();

			if (!isLast)
			{
				this.writer.WriteNewLine();
			}
		}

		private void WriteResultClass(StoredProcedureModel procedure)
		{
			this.writer
				.WriteIndentedLine("public partial class Result")
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				if (procedure.Results.Count == 0)
				{
					this.writer
						.WriteIndentedLine("public int AffectedRows { get; set; }");
				}

				WriteResultOutputProperties(procedure);

				if (procedure.Results.Count > 0)
				{
					WriteResults(procedure);
				}
			}
			WriteBlockEnd();
		}

		private void WriteResultOutputProperties(StoredProcedureModel procedure)
		{
			foreach (var parameter in procedure.OutputParameters)
			{
				this.writer
					.WriteIndentation()
					.Write("public ")
					.Write(parameter.Column.ClrType.TypeName)
					.Write(" ")
					.Write(parameter.Column.PropertyName)
					.Write(" { get; set; }")
					.WriteNewLine();
			}
		}

		private void WriteResults(StoredProcedureModel procedure)
		{
			for (int i = 0; i < procedure.Results.Count; i++)
			{
				this.writer
					.WriteIndentation()
					.Write("public IEnumerable<ResultRow")
					.Write((i + 1).ToString())
					.Write("> Rows")
					.Write((i + 1).ToString())
					.Write(" { get; set; }")
					.WriteNewLine();
			}

			this.writer
				.WriteNewLine();

			for (int i = 0; i < procedure.Results.Count; i++)
			{
				this.writer
					.WriteIndentation()
					.Write("public partial class ResultRow")
					.Write((i + 1).ToString())
					.WriteNewLine()
					.WriteIndentedLine("{");

				this.writer.Indent++;
				{
					StoredProcedureResultModel result = procedure.Results[i];
					foreach (ColumnModel column in result.Columns)
					{
						this.writer
							.WriteIndentation()
							.Write("public ")
							.Write(column.ClrType.TypeName)
							.Write(" ")
							.Write(column.PropertyName)
							.Write(" { get; set; }")
							.WriteNewLine();
					}
				}
				WriteBlockEnd();
			}
		}

		private void WriteParameterClass(StoredProcedureModel procedure)
		{
			if (procedure.Parameters.Count > 0)
			{
				this.writer
					.WriteNewLine()
					.WriteIndentedLine("public partial class Parameters")
					.WriteIndentedLine("{");

				this.writer.Indent++;
				{
					WriteParameterClassConstructor(procedure);

					this.writer.WriteNewLine();

					WriteParameterClassProperties(procedure);
				}
				WriteBlockEnd();
			}
		}

		private void WriteParameterClassConstructor(StoredProcedureModel procedure)
		{
			this.writer
				.WriteIndentation()
				.Write("public Parameters(");

			WriteParameterClassConstructorArguments(procedure);

			this.writer
				.Write(")")
				.WriteNewLine()
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				foreach (var parameter in procedure.Parameters)
				{
					this.writer
						.WriteIndentation()
						.Write("this.")
						.Write(parameter.Column.PropertyName)
						.Write(" = ")
						.Write(parameter.Column.ParameterName)
						.Write(";")
						.WriteNewLine();
				}
			}
			WriteBlockEnd();
		}

		private void WriteParameterClassConstructorArguments(StoredProcedureModel procedure)
		{
			int lastIndex = procedure.Parameters.Count - 1;
			for (int i = 0; i < procedure.Parameters.Count; i++)
			{
				ParameterModel parameter = procedure.Parameters[i];

				this.writer
					.Write(parameter.Column.ClrType.TypeName)
					.Write(" ")
					.Write(parameter.Column.ParameterName);

				if (i != lastIndex)
				{
					this.writer.Write(", ");
				}
			}
		}

		private void WriteParameterClassProperties(StoredProcedureModel procedure)
		{
			foreach (var parameter in procedure.Parameters)
			{
				this.writer
					.WriteIndentation()
					.Write("public ")
					.Write(parameter.Column.ClrType.TypeName)
					.Write(" ")
					.Write(parameter.Column.PropertyName)
					.Write(" { get; private set; }")
					.WriteNewLine();
			}
		}

		private void WriteExecuteMethodOverload(string methodName, StoredProcedureModel procedure)
		{
			this.writer
				.WriteIndentation()
				.Write("public Result ")
				.Write(methodName)
				.Write("(string connectionString");

			if (procedure.Parameters.Count > 0)
			{
				this.writer.Write(", Parameters parameters");
			}

			this.writer
				.Write(")")
				.WriteNewLine()
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				this.writer
					.WriteIndentedLine("using (SqlConnection connection = new SqlConnection(connectionString))")
					.WriteIndentedLine("{");

				this.writer.Indent++;
				{
					this.writer
						.WriteIndentedLine("using (SqlCommand command = new SqlCommand())")
						.WriteIndentedLine("{");

					this.writer.Indent++;
					{
						this.writer
							.WriteIndentedLine("connection.Open();")
							.WriteIndentedLine("command.Connection = connection;")
							.WriteIndentation()
							.Write("return ")
							.Write(methodName)
							.Write("(command");

						if (procedure.Parameters.Count > 0)
						{
							this.writer
								.Write(", parameters");
						}

						this.writer
							.Write(");")
							.WriteNewLine();
					}
					WriteBlockEnd();
				}
				WriteBlockEnd();
			}
			WriteBlockEnd();
		}

		private void WriteExecuteResultMethod(StoredProcedureModel procedure)
		{
			this.writer
				.WriteIndentation()
				.Write("public Result ExecuteResult(SqlCommand c");

			if (procedure.Parameters.Count > 0)
			{
				this.writer.Write(", Parameters parameters");
			}

			this.writer
				.Write(")")
				.WriteNewLine()
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				this.writer
					.WriteIndentation()
					.Write("c.CommandText = \"")
					.Write(procedure.DatabaseName.EscapedFullName)
					.Write("\";")
					.WriteNewLine();

				this.writer
					.WriteIndentation()
					.Write("c.CommandType = CommandType.StoredProcedure;")
					.WriteNewLine();

				if (procedure.Parameters.Count > 0)
				{
					this.writer
						.WriteIndentedLine("SqlParameter p = null;");

					WriteExecuteAddParameters(procedure);
				}

				this.writer
					.WriteIndentedLine("using(var reader = c.ExecuteReader())")
					.WriteIndentedLine("{");

				this.writer.Indent++;
				{
					this.writer
						.WriteIndentedLine("var r = new Result();");

					WriteExecuteMethodRead(procedure);

					WriteExecuteOutputParameters(procedure);

					this.writer
						.WriteIndentedLine("return r;");
				}
				WriteBlockEnd();

			}
			this.WriteBlockEnd();
		}

		private void WriteExecuteMethodRead(StoredProcedureModel procedure)
		{
			for (int i = 0; i < procedure.Results.Count; i++)
			{
				WriteExecuteMethodReadRow(procedure, i);

				if (i < procedure.Results.Count - 1)
				{
					this.writer
						.WriteIndentedLine("reader.NextResult();");
				}
			}
		}

		private void WriteExecuteMethodReadRow(StoredProcedureModel procedure, int i)
		{
			var result = procedure.Results[i];
			string iString = (i + 1).ToString();

			this.writer
				.WriteIndentation()
				.Write("var list")
				.Write(iString)
				.Write("= new List<Result.ResultRow")
				.Write(iString)
				.Write(">();")
				.WriteNewLine();

			this.writer
				.WriteIndentedLine("while (reader.Read())")
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				this.writer
					.WriteIndentation()
					.Write("list")
					.Write(iString)
					.Write(".Add(new Result.ResultRow")
					.Write(iString)
					.Write("()")
					.WriteNewLine()
					.WriteIndentedLine("{");

				this.writer.Indent++;
				{
					WriteExecuteMethodReadValues(result, iString);
				}
				this.writer.Indent--;
				this.writer
					.WriteIndentedLine("});");
			}
			WriteBlockEnd();

			this.writer
				.WriteIndentation()
				.Write("r.Rows")
				.Write(iString)
				.Write(" = list")
				.Write(iString)
				.Write(";")
				.WriteNewLine();
		}

		private void WriteExecuteMethodReadValues(StoredProcedureResultModel result, string iString)
		{
			foreach (var column in result.Columns)
			{
				//Example: FirstName = GetValueOrDefault<string>(reader, "FirstName"),
				this.writer
					.WriteIndentation()
					.Write(column.PropertyName)
					.Write(" = ")
					.Write("GetValueOrDefault<")
					.Write(column.ClrType.TypeName)
					.Write(">(reader, \"")
					.Write(column.DatabaseName)
					.Write("\"),")
					.WriteNewLine();
			}
		}

		private void WriteExecuteNonQueryMethod(StoredProcedureModel procedure)
		{
			this.writer
				.WriteIndentation()
				.Write("public Result ExecuteNonQuery(SqlCommand c");

			if (procedure.Parameters.Count > 0)
			{
				this.writer.Write(", Parameters parameters");
			}

			this.writer
				.Write(")")
				.WriteNewLine()
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				this.writer
					.WriteIndentation()
					.Write("c.CommandText = \"")
					.Write(procedure.DatabaseName.EscapedFullName)
					.Write("\";")
					.WriteNewLine();

				this.writer
					.WriteIndentation()
					.Write("c.CommandType = CommandType.StoredProcedure;")
					.WriteNewLine();

				if (procedure.Parameters.Count > 0)
				{
					this.writer
						.WriteIndentation()
						.Write("SqlParameter p = null;")
						.WriteNewLine();

					WriteExecuteAddParameters(procedure);
				}

				this.writer
					.WriteIndentedLine("var r = new Result();")
					.WriteIndentedLine("r.AffectedRows = c.ExecuteNonQuery();");

				WriteExecuteOutputParameters(procedure);

				this.writer
					.WriteIndentedLine("return r;");
			}
			this.WriteBlockEnd();
		}

		private void WriteExecuteOutputParameters(StoredProcedureModel procedure)
		{
			foreach (var parameter in procedure.OutputParameters)
			{
				this.writer
					.WriteIndentation()
					.Write("r.")
					.Write(parameter.Column.PropertyName)
					.Write(" = (")
					.Write(parameter.Column.ClrType.TypeName)
					.Write(")c.Parameters[\"")
					.Write(parameter.Column.DatabaseName)
					.Write("\"].Value;");
			}
		}

		private void WriteExecuteAddParameters(StoredProcedureModel procedure)
		{
			foreach (var parameter in procedure.Parameters)
			{
				if (parameter.Column.ClrType.IsUserDefined)
				{
					this.writer
						.WriteIndentation()
						.Write("p = c.Parameters.AddWithValue(\"")
						.Write(parameter.Column.DatabaseName)
						.Write("\", parameters.")
						.Write(parameter.Column.PropertyName)
						.Write(".GetDataTable());")
						.WriteNewLine();
				}
				else
				{
					this.writer
						.WriteIndentation()
						.Write("p = c.Parameters.AddWithValue(\"")
						.Write(parameter.Column.DatabaseName)
						.Write("\", parameters.")
						.Write(parameter.Column.PropertyName)
						.Write(");")
						.WriteNewLine();
				}

				this.writer
					.WriteIndentation()
					.Write("p.SqlDbType = SqlDbType.")
					.Write(parameter.SqlDbType.ToString())
					.Write(";")
					.WriteNewLine();

				if (parameter.Size != null)
				{
					this.writer
						.WriteIndentation()
						.Write("p.Size = ")
						.Write(parameter.Size.ToString())
						.Write(";")
						.WriteNewLine();
				}

				if (parameter.Precision != null)
				{
					this.writer
						.WriteIndentation()
						.Write("p.Precision = ")
						.Write(parameter.Precision.ToString())
						.Write(";")
						.WriteNewLine();
				}

				if (parameter.Scale != null)
				{
					this.writer
						.WriteIndentation()
						.Write("p.Scale = ")
						.Write(parameter.Scale.ToString())
						.Write(";")
						.WriteNewLine();
				}

				if (parameter.IsOutput)
				{
					this.writer
						.WriteIndentation()
						.Write("p.Direction = ParameterDirection.Output;")
						.WriteNewLine();
				}

			}

		}

		private void WriteGetValueOrDefaultMethod()
		{
			this.writer.WriteNewLine();
			this.writer.WriteIndentedLine("private static T GetValueOrDefault<T>(SqlDataReader reader, string columnName)");
			this.writer.WriteIndentedLine("{");
			this.writer.Indent++;
			{
				this.writer.WriteIndentedLine("return reader.IsDBNull(reader.GetOrdinal(columnName)) ?");
				this.writer.Indent++;
				{
					this.writer.WriteIndentedLine("default(T) :");
					this.writer.WriteIndentedLine("(T)reader[columnName];");
				}
				this.writer.Indent--; //No block end here
			}
			WriteBlockEnd();
		}

	}

}
