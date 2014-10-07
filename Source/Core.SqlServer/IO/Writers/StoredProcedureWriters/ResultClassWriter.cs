/*
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */

namespace SqlFramework.IO.Writers.StoredProcedureWriters
{
    using System.Globalization;
    using CodeBuilders;
    using Data.Models;

    public class ResultClassWriter : ElementWriterBase
    {
        public ResultClassWriter(ICodeBuilder builder)
            : base(builder)
        {
        }

        public void Write(StoredProcedureModel procedure)
        {
            Builder
                .WriteIndentedLine("public partial class Result");

            WriteBlockStart();
            {
                if (procedure.Results.Count == 0)
                {
                    Builder
                        .WriteIndentedLine("public int AffectedRows { get; set; }");
                }

                WriteOutputProperties(procedure);

                if (procedure.Results.Count > 0)
                {
                    WriteResults(procedure);
                }
            }
            WriteBlockEnd();
        }

        private void WriteOutputProperties(StoredProcedureModel procedure)
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
                    .WriteNewLine();

                WriteBlockStart();
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