namespace SqlFramework.IO.Writers.StoredProcedureWriters
{
    using System.Globalization;
    using CodeBuilders;
    using Data.Models;

    public sealed class SqlServerStoredProcedureWriter : ElementWriterBase
    {
        public SqlServerStoredProcedureWriter(ICodeBuilder builder) : base(builder)
        {
        }

        public void Write(StoredProcedureModel procedure, bool isLast)
        {
            Builder
                .WriteIndentation()
                .Write("public partial class ")
                .Write(procedure.TypeName.Name)
                .WriteNewLine()
                .WriteIndentedLine("{");

            Builder.Indent++;
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

                Builder.WriteNewLine();
                WriteResultClass(procedure);
            }
            WriteBlockEnd();

            if (!isLast)
            {
                Builder.WriteNewLine();
            }
        }

        private void WriteExecuteAddParameters(StoredProcedureModel procedure)
        {
            foreach (var parameter in procedure.Parameters)
            {
                if (parameter.Column.ClrType.IsUserDefined)
                {
                    Builder
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
                    Builder
                        .WriteIndentation()
                        .Write("p = c.Parameters.AddWithValue(\"")
                        .Write(parameter.Column.DatabaseName)
                        .Write("\", parameters.")
                        .Write(parameter.Column.PropertyName)
                        .Write(");")
                        .WriteNewLine();
                }

                Builder
                    .WriteIndentation()
                    .Write("p.SqlDbType = SqlDbType.")
                    .Write(parameter.SqlDbType.ToString())
                    .Write(";")
                    .WriteNewLine();

                if (parameter.Size != null)
                {
                    Builder
                        .WriteIndentation()
                        .Write("p.Size = ")
                        .Write(parameter.Size.ToString())
                        .Write(";")
                        .WriteNewLine();
                }

                if (parameter.Precision != null)
                {
                    Builder
                        .WriteIndentation()
                        .Write("p.Precision = ")
                        .Write(parameter.Precision.ToString())
                        .Write(";")
                        .WriteNewLine();
                }

                if (parameter.Scale != null)
                {
                    Builder
                        .WriteIndentation()
                        .Write("p.Scale = ")
                        .Write(parameter.Scale.ToString())
                        .Write(";")
                        .WriteNewLine();
                }

                if (parameter.IsOutput)
                {
                    Builder
                        .WriteIndentation()
                        .Write("p.Direction = ParameterDirection.Output;")
                        .WriteNewLine();
                }
            }
        }

        private void WriteExecuteMethodOverload(string methodName, StoredProcedureModel procedure)
        {
            Builder
                .WriteIndentation()
                .Write("public Result ")
                .Write(methodName)
                .Write("(string connectionString");

            if (procedure.Parameters.Count > 0)
            {
                Builder.Write(", Parameters parameters");
            }

            Builder
                .Write(")")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                Builder
                    .WriteIndentedLine("using (SqlConnection connection = new SqlConnection(connectionString))")
                    .WriteIndentedLine("{");

                Builder.Indent++;
                {
                    Builder
                        .WriteIndentedLine("using (SqlCommand command = new SqlCommand())")
                        .WriteIndentedLine("{");

                    Builder.Indent++;
                    {
                        Builder
                            .WriteIndentedLine("connection.Open();")
                            .WriteIndentedLine("command.Connection = connection;")
                            .WriteIndentation()
                            .Write("return ")
                            .Write(methodName)
                            .Write("(command");

                        if (procedure.Parameters.Count > 0)
                        {
                            Builder.Write(", parameters");
                        }

                        Builder
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
                    Builder.WriteIndentedLine("reader.NextResult();");
                }
            }
        }

        private void WriteExecuteMethodReadRow(StoredProcedureModel procedure, int i)
        {
            var result = procedure.Results[i];
            string indexString = (i + 1).ToString(CultureInfo.CurrentCulture);

            Builder
                .WriteIndentation()
                .Write("var list")
                .Write(indexString)
                .Write("= new List<Result.ResultRow")
                .Write(indexString)
                .Write(">();")
                .WriteNewLine();

            Builder
                .WriteIndentedLine("while (reader.Read())")
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                Builder
                    .WriteIndentation()
                    .Write("list")
                    .Write(indexString)
                    .Write(".Add(new Result.ResultRow")
                    .Write(indexString)
                    .Write("()")
                    .WriteNewLine()
                    .WriteIndentedLine("{");

                Builder.Indent++;
                {
                    WriteExecuteMethodReadValues(result, indexString);
                }
                Builder.Indent--;
                Builder.WriteIndentedLine("});");
            }
            WriteBlockEnd();

            Builder
                .WriteIndentation()
                .Write("r.Rows")
                .Write(indexString)
                .Write(" = list")
                .Write(indexString)
                .Write(";")
                .WriteNewLine();
        }

        private void WriteExecuteMethodReadValues(StoredProcedureResultModel result, string indexString)
        {
            foreach (var column in result.Columns)
            {
                //Example: FirstName = GetValueOrDefault<string>(reader, "FirstName"),
                Builder
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
            Builder
                .WriteIndentation()
                .Write("public Result ExecuteNonQuery(SqlCommand c");

            if (procedure.Parameters.Count > 0)
            {
                Builder.Write(", Parameters parameters");
            }

            Builder
                .Write(")")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                Builder
                    .WriteIndentation()
                    .Write("c.CommandText = \"")
                    .Write(procedure.DatabaseName.EscapedFullName)
                    .Write("\";")
                    .WriteNewLine();

                Builder
                    .WriteIndentation()
                    .Write("c.CommandType = CommandType.StoredProcedure;")
                    .WriteNewLine();

                if (procedure.Parameters.Count > 0)
                {
                    Builder
                        .WriteIndentation()
                        .Write("SqlParameter p = null;")
                        .WriteNewLine();

                    WriteExecuteAddParameters(procedure);
                }

                Builder
                    .WriteIndentedLine("var r = new Result();")
                    .WriteIndentedLine("r.AffectedRows = c.ExecuteNonQuery();");

                WriteExecuteOutputParameters(procedure);

                Builder
                    .WriteIndentedLine("return r;");
            }
            WriteBlockEnd();
        }

        private void WriteExecuteOutputParameters(StoredProcedureModel procedure)
        {
            foreach (var parameter in procedure.OutputParameters)
            {
                Builder
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
            Builder
                .WriteIndentation()
                .Write("public Result ExecuteResult(SqlCommand c");

            if (procedure.Parameters.Count > 0)
            {
                Builder.Write(", Parameters parameters");
            }

            Builder
                .Write(")")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                Builder
                    .WriteIndentation()
                    .Write("c.CommandText = \"")
                    .Write(procedure.DatabaseName.EscapedFullName)
                    .Write("\";")
                    .WriteNewLine();

                Builder
                    .WriteIndentation()
                    .Write("c.CommandType = CommandType.StoredProcedure;")
                    .WriteNewLine();

                if (procedure.Parameters.Count > 0)
                {
                    Builder
                        .WriteIndentedLine("SqlParameter p = null;");

                    WriteExecuteAddParameters(procedure);
                }

                Builder
                    .WriteIndentedLine("using(var reader = c.ExecuteReader())")
                    .WriteIndentedLine("{");

                Builder.Indent++;
                {
                    Builder.WriteIndentedLine("var r = new Result();");
                    WriteExecuteMethodRead(procedure);
                    WriteExecuteOutputParameters(procedure);
                    Builder.WriteIndentedLine("return r;");
                }
                WriteBlockEnd();
            }
            WriteBlockEnd();
        }

        private void WriteParameterClass(StoredProcedureModel procedure)
        {
            if (procedure.Parameters.Count > 0)
            {
                Builder
                    .WriteNewLine()
                    .WriteIndentedLine("public partial class Parameters")
                    .WriteIndentedLine("{");

                Builder.Indent++;
                {
                    WriteParameterClassConstructor(procedure);
                    Builder.WriteNewLine();
                    WriteParameterClassProperties(procedure);
                }
                WriteBlockEnd();
            }
        }

        private void WriteParameterClassConstructor(StoredProcedureModel procedure)
        {
            Builder
                .WriteIndentation()
                .Write("public Parameters(");

            WriteParameterClassConstructorArguments(procedure);

            Builder
                .Write(")")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                foreach (var parameter in procedure.Parameters)
                {
                    Builder
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

                Builder
                    .Write(parameter.Column.ClrType.TypeName)
                    .Write(" ")
                    .Write(parameter.Column.ParameterName);

                if (i != lastIndex)
                {
                    Builder.Write(", ");
                }
            }
        }

        private void WriteParameterClassProperties(StoredProcedureModel procedure)
        {
            foreach (var parameter in procedure.Parameters)
            {
                Builder
                    .WriteIndentation()
                    .Write("public ")
                    .Write(parameter.Column.ClrType.TypeName)
                    .Write(" ")
                    .Write(parameter.Column.PropertyName)
                    .Write(" { get; private set; }")
                    .WriteNewLine();
            }
        }

        private void WriteResultClass(StoredProcedureModel procedure)
        {
            Builder
                .WriteIndentedLine("public partial class Result")
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                if (procedure.Results.Count == 0)
                {
                    Builder
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
                Builder
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
                Builder
                    .WriteIndentation()
                    .Write("public IEnumerable<ResultRow")
                    .Write((i + 1).ToString(CultureInfo.CurrentCulture))
                    .Write("> Rows")
                    .Write((i + 1).ToString(CultureInfo.CurrentCulture))
                    .Write(" { get; set; }")
                    .WriteNewLine();
            }

            Builder
                .WriteNewLine();

            for (int i = 0; i < procedure.Results.Count; i++)
            {
                Builder
                    .WriteIndentation()
                    .Write("public partial class ResultRow")
                    .Write((i + 1).ToString(CultureInfo.CurrentCulture))
                    .WriteNewLine()
                    .WriteIndentedLine("{");

                Builder.Indent++;
                {
                    StoredProcedureResultModel result = procedure.Results[i];
                    foreach (ColumnModel column in result.Columns)
                    {
                        Builder
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