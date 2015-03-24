/* Example 1: Non query
 * public partial class Result
 * {
 *     public int AffectedRows { get; set; }
 *     
 *     public Guid Id { get; set; }
 * }
 * 
 * Example 2: Query
 * public partial class Result 
 * {
 *     public List<Row1> Rows1 { get; set; }
 *     
 *     public List<Row2> Rows2 { get; set; }
 *     
 *     public class Row1
 *     {
 *         public int Id { get; set; }
 *
 *         public string Name { get; set; }
 *
 *         public int Age { get; set; }
 *     }
 *     
 *     public class Row2
 *     {
 *         public int RelatedId { get; set; }
 *
 *         public SiblingName { get; set; }
 *
 *         public int SiblingAge { get; set; }
 *     }
 * }
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
                    Builder.WriteIndentedLine("public int AffectedRows { get; set; }");
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
            int lastIndex = procedure.Parameters.Count - 1;
            for (int i = 0; i < procedure.OutputParameters.Count; i++)
            {
                var parameter = procedure.OutputParameters[i];
                Builder
                    .WriteIndentation()
                    .Write("public ")
                    .Write(parameter.Column.ClrType.TypeName)
                    .Write(" ")
                    .Write(parameter.Column.PropertyName)
                    .Write(" { get; set; }")
                    .WriteNewLine();

                if(i != lastIndex)
                {
                    Builder.WriteNewLine();
                }
            }
        }

        private void WriteResults(StoredProcedureModel procedure)
        {
            WriteRowsProperties(procedure);

            WriteRowsClasses(procedure);
        }

        private void WriteRowsProperties(StoredProcedureModel procedure)
        {
            for (int i = 0; i < procedure.Results.Count; i++)
            {
                Builder
                    .WriteIndentation()
                    .Write("public List<Row")
                    .Write((i + 1).ToString(CultureInfo.CurrentCulture))
                    .Write("> Rows");

                if (procedure.Results.Count > 1)
                {
                    Builder.Write((i + 1).ToString(CultureInfo.CurrentCulture));
                }

                Builder
                    .Write(" { get; set; }")
                    .WriteNewLine()
                    .WriteNewLine();
            }
        }
        
        private void WriteRowsClasses(StoredProcedureModel procedure)
        {
            int lastIndex = procedure.Parameters.Count - 1;

            for (int i = 0; i < procedure.Results.Count; i++)
            {
                Builder
                    .WriteIndentation()
                    .Write("public partial class Row");

                if (procedure.Results.Count > 1)
                {
                    Builder.Write((i + 1).ToString(CultureInfo.CurrentCulture));
                }

                Builder.WriteNewLine();

                StoredProcedureResultModel result = procedure.Results[i];
                WriteRowClassProperties(result);

                if(i != lastIndex)
                {
                    Builder.WriteNewLine();
                }
            }
        }

        private void WriteRowClassProperties(StoredProcedureResultModel result)
        {
            int lastIndex = result.Columns.Count - 1;
            WriteBlockStart();
            {
                for (int i = 0; i < result.Columns.Count; i++)
                {
                    ColumnModel column = result.Columns[i];
                    Builder
                        .WriteIndentation()
                        .Write("public ")
                        .Write(column.ClrType.TypeName)
                        .Write(" ")
                        .Write(column.PropertyName)
                        .Write(" { get; set; }")
                        .WriteNewLine();

                    if(i != lastIndex)
                    {
                        Builder.WriteNewLine();
                    }
                }
            }
            WriteBlockEnd();
        }
    }
}