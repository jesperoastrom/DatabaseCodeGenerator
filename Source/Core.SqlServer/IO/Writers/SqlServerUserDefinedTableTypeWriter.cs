using System.Collections.Generic;
using System.Linq;
using SqlFramework.Data.Models;
using SqlFramework.IO.CodeBuilders;

namespace SqlFramework.IO.Writers
{
    public sealed class SqlServerUserDefinedTableTypeWriter : ElementWriterBase
    {
        public SqlServerUserDefinedTableTypeWriter(ICodeBuilder builder)
            : base(builder)
        {
        }

        public void Write(SchemaCollection<UserDefinedTableTypeModel> types)
        {
            WriteNamespaceStart(types.ElementNamespace);

            foreach (var schema in types.SchemaElementCollections.OrderBy(s => s.SchemaName))
            {
                BeginWriteStaticClass(schema.SchemaName);
                {
                    List<UserDefinedTableTypeModel> elements = schema.Elements.OrderBy(t => t.DatabaseName).ToList();
                    int lastIndex = elements.Count - 1;

                    for (int i = 0; i < elements.Count; i++)
                    {
                        var element = elements[i];
                        WriteType(element, i == lastIndex);
                    }
                }
                WriteBlockEnd();
            }
            WriteBlockEnd();
        }

        private void WriteAddRowMethod()
        {
            Builder
                .WriteNewLine()
                .WriteIndentedLine("public void AddRow(Row row)")
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                Builder
                    .WriteIndentedLine("this.rows.Add(row);");
            }
            WriteBlockEnd();
        }

        private void WriteConstructor(UserDefinedTableTypeModel type)
        {
            Builder
                .WriteIndentation()
                .Write("public ")
                .Write(type.TypeName.Name)
                .Write("()")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                Builder.WriteIndentedLine("this.rows = new List<Row>();");
            }
            WriteBlockEnd();
        }

        private void WriteGetDataTableMethod(UserDefinedTableTypeModel type)
        {
            Builder
                .WriteNewLine()
                .WriteIndentedLine("public DataTable GetDataTable()")
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                WriteGetDataTableMethodBody(type);
            }
            WriteBlockEnd();
        }

        private void WriteGetDataTableMethodBody(UserDefinedTableTypeModel type)
        {
            Builder
                .WriteIndentedLine("DataTable table = new DataTable();");

            foreach (var column in type.Columns)
            {
                Builder
                    .WriteIndentation()
                    .Write("table.Columns.Add(\"")
                    .Write(column.DatabaseName)
                    .Write("\", typeof(")
                    .Write(column.ClrType.InnerTypeName)
                    .Write("));")
                    .WriteNewLine();
            }

            Builder
                .WriteNewLine()
                .WriteIndentedLine("foreach (Row row in this.rows)")
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                Builder.WriteIndentedLine("DataRow dataRow = table.NewRow();");

                foreach (var column in type.Columns)
                {
                    if (column.ClrType.IsNullable)
                    {
                        Builder
                            .WriteIndentation()
                            .Write("dataRow[\"")
                            .Write(column.DatabaseName)
                            .Write("\"] = row.")
                            .Write(column.PropertyName)
                            .Write(".HasValue ? (object)row.")
                            .Write(column.PropertyName)
                            .Write(".Value : DBNull.Value;")
                            .WriteNewLine();
                    }
                    else
                    {
                        Builder
                            .WriteIndentation()
                            .Write("dataRow[\"")
                            .Write(column.DatabaseName)
                            .Write("\"] = row.")
                            .Write(column.PropertyName)
                            .Write(";")
                            .WriteNewLine();
                    }
                }
                Builder.WriteIndentedLine("table.Rows.Add(dataRow);");
            }
            WriteBlockEnd();

            Builder.WriteIndentedLine("return table;");
        }

        private void WritePrivateMembers()
        {
            Builder
                .WriteNewLine()
                .WriteIndentedLine("private readonly List<Row> rows;");
        }

        private void WriteRowClass(UserDefinedTableTypeModel type)
        {
            Builder
                .WriteNewLine()
                .WriteIndentedLine("public partial class Row")
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                WriteRowClassBody(type);
            }
            WriteBlockEnd();
        }

        private void WriteRowClassBody(UserDefinedTableTypeModel type)
        {
            Builder
                .WriteIndentation()
                .Write("public Row(");

            WriteRowClassConstructorArgumentList(type);

            Builder
                .Write(")")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                WriteRowClassConstructorBody(type);
            }
            WriteBlockEnd();

            WriteRowClassProperties(type);
        }

        private void WriteRowClassConstructorArgumentList(UserDefinedTableTypeModel type)
        {
            int lastIndex = type.Columns.Count - 1;
            for (int i = 0; i < type.Columns.Count; i++)
            {
                var column = type.Columns[i];

                Builder
                    .Write(column.ClrType.TypeName)
                    .Write(" ")
                    .Write(column.ParameterName);

                if (i != lastIndex)
                {
                    Builder.Write(", ");
                }
            }
        }

        private void WriteRowClassConstructorBody(UserDefinedTableTypeModel type)
        {
            foreach (var column in type.Columns)
            {
                Builder
                    .WriteIndentation()
                    .Write("this.")
                    .Write(column.PropertyName)
                    .Write(" = ")
                    .Write(column.ParameterName)
                    .Write(";")
                    .WriteNewLine();
            }
        }

        private void WriteRowClassProperties(UserDefinedTableTypeModel type)
        {
            foreach (var column in type.Columns)
            {
                Builder
                    .WriteIndentation()
                    .Write("public ")
                    .Write(column.ClrType.TypeName)
                    .Write(" ")
                    .Write(column.PropertyName)
                    .Write(" { get; private set; }")
                    .WriteNewLine();
            }
        }

        private void WriteType(UserDefinedTableTypeModel type, bool isLast)
        {
            Builder
                .WriteIndentation()
                .Write("public partial class ")
                .Write(type.TypeName.Name)
                .WriteNewLine()
                .WriteIndentedLine("{");

            Builder.Indent++;
            {
                WriteConstructor(type);
                WriteAddRowMethod();
                WriteGetDataTableMethod(type);
                WritePrivateMembers();
                WriteRowClass(type);
            }
            WriteBlockEnd();

            if (!isLast)
            {
                Builder.WriteNewLine();
            }
        }
    }
}