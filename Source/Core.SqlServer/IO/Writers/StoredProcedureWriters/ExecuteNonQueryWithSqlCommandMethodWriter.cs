namespace SqlFramework.IO.Writers.StoredProcedureWriters
{
    using CodeBuilders;
    using Data.Models;

    public class ExecuteNonQueryWithSqlCommandMethodWriter : ExecuteUsingCommandWriter
    {
        public ExecuteNonQueryWithSqlCommandMethodWriter(ICodeBuilder builder)
            : base(builder)
        {
        }

        public void Write(StoredProcedureModel procedure)
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
                        .WriteIndentation()
                        .Write("SqlParameter p = null;")
                        .WriteNewLine();

                    WriteAddParameters(procedure);
                }

                Builder
                    .WriteIndentedLine("var r = new Result();")
                    .WriteIndentedLine("r.AffectedRows = c.ExecuteNonQuery();");

                WriteOutputParameters(procedure);

                Builder
                    .WriteIndentedLine("return r;");
            }
            WriteBlockEnd();
        }
    }
}