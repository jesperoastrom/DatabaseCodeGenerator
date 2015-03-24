/* Example: 
 * public Result ExecuteResult(SqlCommand command, Parameters parameters)
 * {
 *     command.CommandText = "[dbo].[GetOrders]";
 *     command.CommandType = CommandType.StoredProcedure;
 *     
 *     SqlParameter parameter = null;
 *     <Write Add Parameters>
 *     
 *     using(var reader = command.ExecuteReader())
 *     {
 *         var result = new Result();
 *         result.Rows1 = new List<Result.Row>();
 *         while (reader.Read())
 *         {
 *             result.Rows1.Add(new Row1()
 *             {
 *                 Id = reader.GetValueOrDefault<Guid>("Id"),
 *                 Name = reader.GetValueOrDefault<string>("Name"),
 *                 Age = reader.GetValueOrDefault<int>("Age")
 *             });
 *         }
 *         reader.NextResult();
 *         while (reader.Read())
 *         {
 *             result.Rows2.Add(new Row2()
 *             {
 *                 RelatedId = reader.GetValueOrDefault<Guid>("RelatedId"),
 *                 SiblingName = reader.GetValueOrDefault<string>("SiblingName"),
 *                 SiblingAge = reader.GetValueOrDefault<int>("SiblingAge")
 *             });
 *         }         
 *         return result;
 *     }
 * }
 * */

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
                .Write("public Result ExecuteResult(SqlCommand command");

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
                    .WriteNewLine()
                    .WriteNewLine();

                if (procedure.Parameters.Count > 0)
                {
                    Builder.WriteIndentedLine("SqlParameter parameter = null;");

                    WriteAddParameters(procedure);
                }

                Builder.WriteIndentedLine("using(var reader = command.ExecuteReader())");

                WriteBlockStart();
                {
                    Builder.WriteIndentedLine("var result = new Result();");
                    WriteReadingResults(procedure);
                    WriteOutputParameters(procedure);
                    Builder.WriteIndentedLine("return result;");
                }
                WriteBlockEnd();
            }
            WriteBlockEnd();
        }

        private void WriteReadingResults(StoredProcedureModel procedure)
        {
            int lastIndex = procedure.Results.Count - 1;
            for (int i = 0; i < procedure.Results.Count; i++)
            {
                WriteReadingRows(procedure, i);
                
                if (i != lastIndex)
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
                .Write("result.Rows")
                .Write(indexString)
                .Write("= new List<Result.Row")
                .Write(indexString)
                .Write(">();")
                .WriteNewLine();

            Builder
                .WriteIndentedLine("while (reader.Read())");

            WriteBlockStart();
            {
                Builder
                    .WriteIndentation()
                    .Write("result.Rows")
                    .Write(indexString)
                    .Write(".Add(new Result.Row")
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
        }

        private void WriteExecuteMethodReadValues(StoredProcedureResultModel result)
        {
            foreach (var column in result.Columns)
            {
                
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