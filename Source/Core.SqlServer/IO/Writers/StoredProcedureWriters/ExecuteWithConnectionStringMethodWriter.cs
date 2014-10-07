/* Example:
 * public Result ExecuteResult(string connectionString, Parameters parameters)
 * {
 *    using (SqlConnection connection = new SqlConnection(connectionString))
 *    {
 *        using (SqlCommand command = new SqlCommand())
 *        {
 *            connection.Open();
 *            command.Connection = connection;
 *            return ExecuteResult(command, parameters);
 *        }
 *    }
 * }
 */
namespace SqlFramework.IO.Writers.StoredProcedureWriters
{
    using CodeBuilders;
    using Data.Models;

    public sealed class ExecuteWithConnectionStringMethodWriter : ElementWriterBase
    {
        public ExecuteWithConnectionStringMethodWriter(ICodeBuilder builder)
            : base(builder)
        {
        }

        public void Write(string methodName, StoredProcedureModel procedure)
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
                .WriteNewLine();

            WriteBlockStart();
            {
                Builder
                    .WriteIndentedLine("using (SqlConnection connection = new SqlConnection(connectionString))");

                WriteBlockStart();
                {
                    Builder
                        .WriteIndentedLine("using (SqlCommand command = new SqlCommand())");

                    WriteBlockStart();
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
    }
}