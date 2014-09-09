using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SqlFramework.Data.Models;
using SqlFramework.IO;

namespace SqlFramework.Data.SqlServer2012.IO
{
    public sealed class SqlServerStoredProcedureWriter : ElementWriterBase, IStoredProcedureWriter
    {
        public SqlServerStoredProcedureWriter(ICodeWriter writer)
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

        private void WriteExecuteAddParameters(StoredProcedureModel procedure)
        {
            foreach (var parameter in procedure.Parameters)
            {
                if (parameter.Column.ClrType.IsUserDefined)
                {
                    Writer
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
                    Writer
                        .WriteIndentation()
                        .Write("p = c.Parameters.AddWithValue(\"")
                        .Write(parameter.Column.DatabaseName)
                        .Write("\", parameters.")
                        .Write(parameter.Column.PropertyName)
                        .Write(");")
                        .WriteNewLine();
                }

                Writer
                    .WriteIndentation()
                    .Write("p.SqlDbType = SqlDbType.")
                    .Write(parameter.SqlDbType.ToString())
                    .Write(";")
                    .WriteNewLine();

                if (parameter.Size != null)
                {
                    Writer
                        .WriteIndentation()
                        .Write("p.Size = ")
                        .Write(parameter.Size.ToString())
                        .Write(";")
                        .WriteNewLine();
                }

                if (parameter.Precision != null)
                {
                    Writer
                        .WriteIndentation()
                        .Write("p.Precision = ")
                        .Write(parameter.Precision.ToString())
                        .Write(";")
                        .WriteNewLine();
                }

                if (parameter.Scale != null)
                {
                    Writer
                        .WriteIndentation()
                        .Write("p.Scale = ")
                        .Write(parameter.Scale.ToString())
                        .Write(";")
                        .WriteNewLine();
                }

                if (parameter.IsOutput)
                {
                    Writer
                        .WriteIndentation()
                        .Write("p.Direction = ParameterDirection.Output;")
                        .WriteNewLine();
                }
            }
        }


        private void WriteExecuteMethodOverload(string methodName, StoredProcedureModel procedure)
        {
            Writer
                .WriteIndentation()
                .Write("public Result ")
                .Write(methodName)
                .Write("(string connectionString");

            if (procedure.Parameters.Count > 0)
            {
                Writer.Write(", Parameters parameters");
            }

            Writer
                .Write(")")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Writer.Indent++;
            {
                Writer
                    .WriteIndentedLine("using (SqlConnection connection = new SqlConnection(connectionString))")
                    .WriteIndentedLine("{");

                Writer.Indent++;
                {
                    Writer
                        .WriteIndentedLine("using (SqlCommand command = new SqlCommand())")
                        .WriteIndentedLine("{");

                    Writer.Indent++;
                    {
                        Writer
                            .WriteIndentedLine("connection.Open();")
                            .WriteIndentedLine("command.Connection = connection;")
                            .WriteIndentation()
                            .Write("return ")
                            .Write(methodName)
                            .Write("(command");

                        if (procedure.Parameters.Count > 0)
                        {
                            Writer.Write(", parameters");
                        }

                        Writer
                            .Write(");")
                            .WriteNewLine();
                    }
                    WriteBlockEnd();
                }
                WriteBlockEnd();
            }
            WriteBlockEnd();
        }

        private void WriteExecuteMethodRead(StoredProcedureModel procedure)
        {
            for (int i = 0; i < procedure.Results.Count; i++)
            {
                WriteExecuteMethodReadRow(procedure, i);

                if (i < procedure.Results.Count - 1)
                {
                    Writer.WriteIndentedLine("reader.NextResult();");
                }
            }
        }

        private void WriteExecuteMethodReadRow(StoredProcedureModel procedure, int i)
        {
            var result = procedure.Results[i];
            string iString = (i + 1).ToString(CultureInfo.CurrentCulture);

            Writer
                .WriteIndentation()
                .Write("var list")
                .Write(iString)
                .Write("= new List<Result.ResultRow")
                .Write(iString)
                .Write(">();")
                .WriteNewLine();

            Writer
                .WriteIndentedLine("while (reader.Read())")
                .WriteIndentedLine("{");

            Writer.Indent++;
            {
                Writer
                    .WriteIndentation()
                    .Write("list")
                    .Write(iString)
                    .Write(".Add(new Result.ResultRow")
                    .Write(iString)
                    .Write("()")
                    .WriteNewLine()
                    .WriteIndentedLine("{");

                Writer.Indent++;
                {
                    WriteExecuteMethodReadValues(result, iString);
                }
                Writer.Indent--;
                Writer.WriteIndentedLine("});");
            }
            WriteBlockEnd();

            Writer
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
                Writer
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
            Writer
                .WriteIndentation()
                .Write("public Result ExecuteNonQuery(SqlCommand c");

            if (procedure.Parameters.Count > 0)
            {
                Writer.Write(", Parameters parameters");
            }

            Writer
                .Write(")")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Writer.Indent++;
            {
                Writer
                    .WriteIndentation()
                    .Write("c.CommandText = \"")
                    .Write(procedure.DatabaseName.EscapedFullName)
                    .Write("\";")
                    .WriteNewLine();

                Writer
                    .WriteIndentation()
                    .Write("c.CommandType = CommandType.StoredProcedure;")
                    .WriteNewLine();

                if (procedure.Parameters.Count > 0)
                {
                    Writer
                        .WriteIndentation()
                        .Write("SqlParameter p = null;")
                        .WriteNewLine();

                    WriteExecuteAddParameters(procedure);
                }

                Writer
                    .WriteIndentedLine("var r = new Result();")
                    .WriteIndentedLine("r.AffectedRows = c.ExecuteNonQuery();");

                WriteExecuteOutputParameters(procedure);

                Writer
                    .WriteIndentedLine("return r;");
            }
            WriteBlockEnd();
        }

        private void WriteExecuteOutputParameters(StoredProcedureModel procedure)
        {
            foreach (var parameter in procedure.OutputParameters)
            {
                Writer
                    .WriteIndentation()
                    .Write("r.")
                    .Write(parameter.Column.PropertyName)
                    .Write(" = (")
                    .Write(parameter.Column.ClrType.TypeName)
                    .Write(")c.Parameters[\"")
                    .Write(parameter.Column.DatabaseName)
                    .Write("\"].Value;")
                    .WriteNewLine();
            }
        }

        private void WriteExecuteResultMethod(StoredProcedureModel procedure)
        {
            Writer
                .WriteIndentation()
                .Write("public Result ExecuteResult(SqlCommand c");

            if (procedure.Parameters.Count > 0)
            {
                Writer.Write(", Parameters parameters");
            }

            Writer
                .Write(")")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Writer.Indent++;
            {
                Writer
                    .WriteIndentation()
                    .Write("c.CommandText = \"")
                    .Write(procedure.DatabaseName.EscapedFullName)
                    .Write("\";")
                    .WriteNewLine();

                Writer
                    .WriteIndentation()
                    .Write("c.CommandType = CommandType.StoredProcedure;")
                    .WriteNewLine();

                if (procedure.Parameters.Count > 0)
                {
                    Writer
                        .WriteIndentedLine("SqlParameter p = null;");

                    WriteExecuteAddParameters(procedure);
                }

                Writer
                    .WriteIndentedLine("using(var reader = c.ExecuteReader())")
                    .WriteIndentedLine("{");

                Writer.Indent++;
                {
                    Writer.WriteIndentedLine("var r = new Result();");
                    WriteExecuteMethodRead(procedure);
                    WriteExecuteOutputParameters(procedure);
                    Writer.WriteIndentedLine("return r;");
                }
                WriteBlockEnd();
            }
            WriteBlockEnd();
        }

        private void WriteGetValueOrDefaultMethod()
        {
            Writer.WriteNewLine();
            Writer.WriteIndentedLine("private static T GetValueOrDefault<T>(SqlDataReader reader, string columnName)");
            Writer.WriteIndentedLine("{");
            Writer.Indent++;
            {
                Writer.WriteIndentedLine("return reader.IsDBNull(reader.GetOrdinal(columnName)) ?");
                Writer.Indent++;
                {
                    Writer.WriteIndentedLine("default(T) :");
                    Writer.WriteIndentedLine("(T)reader[columnName];");
                }
                Writer.Indent--; //No block end here
            }
            WriteBlockEnd();
        }

        private void WriteParameterClass(StoredProcedureModel procedure)
        {
            if (procedure.Parameters.Count > 0)
            {
                Writer
                    .WriteNewLine()
                    .WriteIndentedLine("public partial class Parameters")
                    .WriteIndentedLine("{");

                Writer.Indent++;
                {
                    WriteParameterClassConstructor(procedure);
                    Writer.WriteNewLine();
                    WriteParameterClassProperties(procedure);
                }
                WriteBlockEnd();
            }
        }

        private void WriteParameterClassConstructor(StoredProcedureModel procedure)
        {
            Writer
                .WriteIndentation()
                .Write("public Parameters(");

            WriteParameterClassConstructorArguments(procedure);

            Writer
                .Write(")")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Writer.Indent++;
            {
                foreach (var parameter in procedure.Parameters)
                {
                    Writer
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

                Writer
                    .Write(parameter.Column.ClrType.TypeName)
                    .Write(" ")
                    .Write(parameter.Column.ParameterName);

                if (i != lastIndex)
                {
                    Writer.Write(", ");
                }
            }
        }

        private void WriteParameterClassProperties(StoredProcedureModel procedure)
        {
            foreach (var parameter in procedure.Parameters)
            {
                Writer
                    .WriteIndentation()
                    .Write("public ")
                    .Write(parameter.Column.ClrType.TypeName)
                    .Write(" ")
                    .Write(parameter.Column.PropertyName)
                    .Write(" { get; private set; }")
                    .WriteNewLine();
            }
        }

        private void WriteProcedure(StoredProcedureModel procedure, bool isLast)
        {
            Writer
                .WriteIndentation()
                .Write("public partial class ")
                .Write(procedure.TypeName.Name)
                .WriteNewLine()
                .WriteIndentedLine("{");


            Writer.Indent++;
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

                Writer.WriteNewLine();
                WriteResultClass(procedure);
            }
            WriteBlockEnd();

            if (!isLast)
            {
                Writer.WriteNewLine();
            }
        }

        private void WriteResultClass(StoredProcedureModel procedure)
        {
            Writer
                .WriteIndentedLine("public partial class Result")
                .WriteIndentedLine("{");

            Writer.Indent++;
            {
                if (procedure.Results.Count == 0)
                {
                    Writer
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
                Writer
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
                Writer
                    .WriteIndentation()
                    .Write("public IEnumerable<ResultRow")
                    .Write((i + 1).ToString(CultureInfo.CurrentCulture))
                    .Write("> Rows")
                    .Write((i + 1).ToString(CultureInfo.CurrentCulture))
                    .Write(" { get; set; }")
                    .WriteNewLine();
            }

            Writer
                .WriteNewLine();

            for (int i = 0; i < procedure.Results.Count; i++)
            {
                Writer
                    .WriteIndentation()
                    .Write("public partial class ResultRow")
                    .Write((i + 1).ToString(CultureInfo.CurrentCulture))
                    .WriteNewLine()
                    .WriteIndentedLine("{");

                Writer.Indent++;
                {
                    StoredProcedureResultModel result = procedure.Results[i];
                    foreach (ColumnModel column in result.Columns)
                    {
                        Writer
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
    }
}