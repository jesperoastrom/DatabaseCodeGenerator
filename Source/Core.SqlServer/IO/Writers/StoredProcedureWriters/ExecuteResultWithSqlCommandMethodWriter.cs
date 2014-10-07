namespace SqlFramework.IO.Writers.StoredProcedureWriters
{
    using System.Globalization;
    using CodeBuilders;
    using Data.Models;

    public sealed class ExecuteResultWithSqlCommandMethodWriter : ExecuteUsingCommandWriter
    {
        public ExecuteResultWithSqlCommandMethodWriter(ICodeBuilder builder) : base(builder)
        {
        }

        public void Write(StoredProcedureModel procedure)
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
                .WriteNewLine();

            WriteBlockStart();
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

                    WriteAddParameters(procedure);
                }

                Builder
                    .WriteIndentedLine("using(var reader = c.ExecuteReader())");

                WriteBlockStart();
                {
                    Builder.WriteIndentedLine("var r = new Result();");
                    WriteReadingResults(procedure);
                    WriteOutputParameters(procedure);
                    Builder.WriteIndentedLine("return r;");
                }
                WriteBlockEnd();
            }
            WriteBlockEnd();
        }

        private void WriteReadingResults(StoredProcedureModel procedure)
        {
            for (int i = 0; i < procedure.Results.Count; i++)
            {
                WriteReadingRows(procedure, i);

                if (i < procedure.Results.Count - 1)
                {
                    Builder.WriteIndentedLine("reader.NextResult();");
                }
            }
        }

        private void WriteReadingRows(StoredProcedureModel procedure, int i)
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
                .WriteIndentedLine("while (reader.Read())");

            WriteBlockStart();
            {
                Builder
                    .WriteIndentation()
                    .Write("list")
                    .Write(indexString)
                    .Write(".Add(new Result.ResultRow")
                    .Write(indexString)
                    .Write("()")
                    .WriteNewLine();

                WriteBlockStart();
                {
                    WriteExecuteMethodReadValues(result);
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

        private void WriteExecuteMethodReadValues(StoredProcedureResultModel result)
        {
            foreach (var column in result.Columns)
            {
                //Example: FirstName = GetValueOrDefault<string>(reader, "Name"),
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
    }
}