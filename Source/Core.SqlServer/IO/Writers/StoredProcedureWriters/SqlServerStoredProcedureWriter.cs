namespace SqlFramework.IO.Writers.StoredProcedureWriters
{
    using CodeBuilders;
    using Data.Models;

    public sealed class SqlServerStoredProcedureWriter : ElementWriterBase
    {
        public SqlServerStoredProcedureWriter(ICodeBuilder builder)
            : base(builder)
        {
            _parameterClassWriter = new ParameterClassWriter(builder);
            _executeWithConnectionStringMethodWriter = new ExecuteWithConnectionStringMethodWriter(builder);
            _executeResultUsingSqlCommandMethodWriter = new ExecuteResultWithSqlCommandMethodWriter(builder);
            _executeNonQueryWithSqlCommandMethodWriter = new ExecuteNonQueryWithSqlCommandMethodWriter(builder);
            _resultClassWriter = new ResultClassWriter(builder);
        }

        public void Write(StoredProcedureModel procedure, bool isLast)
        {
            Builder
                .WriteIndentation()
                .Write("public partial class ")
                .Write(procedure.TypeName.Name)
                .WriteNewLine();

            WriteBlockStart();
            {
                if (procedure.Results.Count > 0)
                {
                    _executeWithConnectionStringMethodWriter.Write("ExecuteResult", procedure);
                    _executeResultUsingSqlCommandMethodWriter.Write(procedure);
                }
                else
                {
                    _executeWithConnectionStringMethodWriter.Write("ExecuteNonQuery", procedure);
                    _executeNonQueryWithSqlCommandMethodWriter.Write(procedure);
                }

                _parameterClassWriter.Write(procedure);

                Builder.WriteNewLine();
                _resultClassWriter.Write(procedure);
            }
            WriteBlockEnd();

            if (!isLast)
            {
                Builder.WriteNewLine();
            }
        }

        private readonly ExecuteNonQueryWithSqlCommandMethodWriter _executeNonQueryWithSqlCommandMethodWriter;
        private readonly ExecuteResultWithSqlCommandMethodWriter _executeResultUsingSqlCommandMethodWriter;
        private readonly ExecuteWithConnectionStringMethodWriter _executeWithConnectionStringMethodWriter;
        private readonly ParameterClassWriter _parameterClassWriter;
        private readonly ResultClassWriter _resultClassWriter;
    }
}