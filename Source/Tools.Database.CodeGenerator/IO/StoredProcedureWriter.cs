using System;
using System.Collections.Generic;
using System.Linq;
using Flip.Tools.Database.CodeGenerator.Data.Models;
using System.Data.SqlClient;
using System.Data;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	internal sealed class StoredProcedureWriter
	{

		public StoredProcedureWriter(Writer writer)
		{
			this.writer = writer;
		}



		public void Write(SchemaCollection<StoredProcedureModel> storedProcedures)
		{
			WriteUsings();
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
				}
				WriteBlockEnd();
			}

			WriteBlockEnd();
		}



		private void BeginWriteStaticClass(string className)
		{
			this.writer
				.WriteIndentation()
				.Write("public static class ")
				.Write(className)
				.WriteNewLine()
				.WriteIndentedLine("{")
				.WriteNewLine();

			this.writer.Indent++;
		}

		private void WriteUsings()
		{
			this.writer
				.WriteIndentedLine("using System;")
				.WriteIndentedLine("using System.Data;")
				.WriteIndentedLine("using System.Data.SqlClient;")
				.WriteNewLine()
				.WriteNewLine()
				.WriteNewLine();
		}

		private void WriteNamespaceStart(string ns)
		{
			if (string.IsNullOrEmpty(ns))
			{
				throw new ArgumentException("Namespace may not be empty");
			}

			this.writer
				.Write("namespace ")
				.Write(ns)
				.WriteNewLine()
				.WriteIndentedLine("{")
				.WriteNewLine();

			this.writer.Indent++;
		}

		private void WriteBlockEnd(bool newLine = true)
		{
			this.writer.Indent--;

			if (newLine)
			{
				this.writer.WriteNewLine();
			}
			this.writer.WriteIndentedLine("}");
		}

		private void WriteProcedure(StoredProcedureModel procedure, bool isLast)
		{
			this.writer
				.WriteIndentation()
				.Write("public partial class ")
				.Write(procedure.TypeName.Name)
				.WriteNewLine()
				.WriteIndentedLine("{")
				.WriteNewLine();

			this.writer.Indent++;
			{
				WriteExecuteMethod(procedure);

				writer.WriteNewLine();

				WriteParameterClass(procedure);
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
				//TODO
			}
			WriteBlockEnd(false);
		}

		private void WriteParameterClass(StoredProcedureModel procedure)
		{
			if (procedure.Parameters.Count > 0)
			{
				this.writer
					.WriteIndentedLine("public partial class Parameters")
					.WriteIndentedLine("{");

				this.writer.Indent++;
				{
					WriteParameterClassConstructor(procedure);

					this.writer.WriteNewLine();

					WriteParameterClassProperties(procedure);
				}
				WriteBlockEnd(false);

				this.writer.WriteNewLine();
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
			WriteBlockEnd(false);
		}

		private void WriteParameterClassConstructorArguments(StoredProcedureModel procedure)
		{
			int lastIndex = procedure.Parameters.Count - 1;
			for (int i = 0; i < procedure.Parameters.Count; i++)
			{
				ParameterModel parameter = procedure.Parameters[i];

				this.writer
					.Write(parameter.Column.ClrType)
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
					.Write(parameter.Column.ClrType)
					.Write(" ")
					.Write(parameter.Column.PropertyName)
					.Write(" { get; private set; }")
					.WriteNewLine();
			}
		}

		private void WriteExecuteMethod(StoredProcedureModel procedure)
		{
			this.writer
				.WriteIndentation()
				.Write("public Result ExecuteResult(SqlCommand command");

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
					.Write("command.CommandText = \"")
					.Write(procedure.DatabaseName.EscapedFullName)
					.Write("\";")
					.WriteNewLine();

				this.writer
					.WriteIndentation()
					.Write("command.CommandType = CommandType.StoredProcedure;")
					.WriteNewLine();

				if (procedure.Parameters.Count > 0)
				{
					this.writer
						.WriteIndentation()
						.Write("SqlParameter p = null;")
						.WriteNewLine();

					WriteExecuteParameters(procedure);
				}

				this.writer
					.WriteIndentation()
					.Write("using(var reader = command.ExecuteReader())")
					.WriteNewLine()
					.WriteIndentedLine("{");

				this.writer.Indent++;
				{
					//TODO
				}
				WriteBlockEnd();

			}
			this.WriteBlockEnd();
		}

		private void WriteExecuteParameters(StoredProcedureModel procedure)
		{
			foreach (var parameter in procedure.Parameters)
			{
				this.writer
					.WriteIndentation()
					.Write("p = new SqlParameter(\"")
					.Write(parameter.Column.DatabaseName)
					.Write("\", SqlDbType.")
					.Write(parameter.SqlDbType.ToString())
					.Write(");")
					.WriteNewLine();

				if (parameter.IsOutput)
				{
					this.writer
						.WriteIndentation()
						.Write("p.Direction = ParameterDirection.Output;")
						.WriteNewLine();
				}

				this.writer
					.WriteIndentation()
					.Write("p.Value = parameters.")
					.Write(parameter.Column.PropertyName)
					.Write(";")
					.WriteNewLine();

				this.writer
					.WriteIndentation()
					.Write("command.Parameters.Add(p);")
					.WriteNewLine();
			}
		}

		private void Test()
		{
			//SqlCommand c;
			//c.CommandText = "";
			//c.CommandType = CommandType.StoredProcedure;
			SqlParameter p;
			//p.Precision
			//p.Scale
			//p.Size
			//p.UdtTypeName
			//p.Value
			//p.
		}



		private readonly Writer writer;

	}

}
