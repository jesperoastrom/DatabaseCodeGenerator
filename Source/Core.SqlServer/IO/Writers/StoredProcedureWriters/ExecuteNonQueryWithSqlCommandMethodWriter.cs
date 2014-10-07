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
                .Write("public Result ExecuteNonQuery(SqlCommand command");

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
                    .Write("command.CommandText = \"")
                    .Write(procedure.DatabaseName.EscapedFullName)
                    .Write("\";")
                    .WriteNewLine();

                Builder
                    .WriteIndentation()
                    .Write("command.CommandType = CommandType.StoredProcedure;")
                    .WriteNewLine();

                if (procedure.Parameters.Count > 0)
                {
                    Builder
                        .WriteIndentation()
                        .Write("SqlParameter parameter = null;")
                        .WriteNewLine();

                    WriteAddParameters(procedure);
                }

                Builder
                    .WriteIndentedLine("var result = new Result();")
                    .WriteIndentedLine("result.AffectedRows = command.ExecuteNonQuery();");

                WriteOutputParameters(procedure);

                Builder
                    .WriteIndentedLine("return result;");
            }
            WriteBlockEnd();
        }
    }
}