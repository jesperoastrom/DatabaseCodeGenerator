using System.Collections.Generic;
using System.Linq;
using SqlFramework.Data.Models;
using SqlFramework.IO;

namespace SqlFramework.Data.SqlServer2012.IO
{
    public sealed class SqlServerUserDefinedTableTypeWriter : ElementWriterBase
    {
        public SqlServerUserDefinedTableTypeWriter(ICodeWriter writer)
            : base(writer)
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
            Writer
                .WriteNewLine()
                .WriteIndentedLine("public void AddRow(Row row)")
                .WriteIndentedLine("{");

            Writer.Indent++;
            {
                Writer
                    .WriteIndentedLine("this.rows.Add(row);");
            }
            WriteBlockEnd();
        }

        private void WriteConstructor(UserDefinedTableTypeModel type)
        {
            Writer
                .WriteIndentation()
                .Write("public ")
                .Write(type.TypeName.Name)
                .Write("()")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Writer.Indent++;
            {
                Writer.WriteIndentedLine("this.rows = new List<Row>();");
            }
            WriteBlockEnd();
        }

        private void WriteGetDataTableMethod(UserDefinedTableTypeModel type)
        {
            Writer
                .WriteNewLine()
                .WriteIndentedLine("public DataTable GetDataTable()")
                .WriteIndentedLine("{");

            Writer.Indent++;
            {
                WriteGetDataTableMethodBody(type);
            }
            WriteBlockEnd();
        }

        private void WriteGetDataTableMethodBody(UserDefinedTableTypeModel type)
        {
            Writer
                .WriteIndentedLine("DataTable table = new DataTable();");

            foreach (var column in type.Columns)
            {
                Writer
                    .WriteIndentation()
                    .Write("table.Columns.Add(\"")
                    .Write(column.DatabaseName)
                    .Write("\", typeof(")
                    .Write(column.ClrType.InnerTypeName)
                    .Write("));")
                    .WriteNewLine();
            }

            Writer
                .WriteNewLine()
                .WriteIndentedLine("foreach (Row row in this.rows)")
                .WriteIndentedLine("{");

            Writer.Indent++;
            {
                Writer.WriteIndentedLine("DataRow dataRow = table.NewRow();");

                foreach (var column in type.Columns)
                {
                    if (column.ClrType.IsNullable)
                    {
                        Writer
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
                        Writer
                            .WriteIndentation()
                            .Write("dataRow[\"")
                            .Write(column.DatabaseName)
                            .Write("\"] = row.")
                            .Write(column.PropertyName)
                            .Write(";")
                            .WriteNewLine();
                    }
                }
                Writer.WriteIndentedLine("table.Rows.Add(dataRow);");
            }
            WriteBlockEnd();

            Writer.WriteIndentedLine("return table;");
        }

        private void WritePrivateMembers()
        {
            Writer
                .WriteNewLine()
                .WriteIndentedLine("private readonly List<Row> rows;");
        }

        private void WriteRowClass(UserDefinedTableTypeModel type)
        {
            Writer
                .WriteNewLine()
                .WriteIndentedLine("public partial class Row")
                .WriteIndentedLine("{");

            Writer.Indent++;
            {
                WriteRowClassBody(type);
            }
            WriteBlockEnd();
        }

        private void WriteRowClassBody(UserDefinedTableTypeModel type)
        {
            Writer
                .WriteIndentation()
                .Write("public Row(");

            WriteRowClassConstructorArgumentList(type);

            Writer
                .Write(")")
                .WriteNewLine()
                .WriteIndentedLine("{");

            Writer.Indent++;
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

                Writer
                    .Write(column.ClrType.TypeName)
                    .Write(" ")
                    .Write(column.ParameterName);

                if (i != lastIndex)
                {
                    Writer.Write(", ");
                }
            }
        }

        private void WriteRowClassConstructorBody(UserDefinedTableTypeModel type)
        {
            foreach (var column in type.Columns)
            {
                Writer
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
                Writer
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
            Writer
                .WriteIndentation()
                .Write("public partial class ")
                .Write(type.TypeName.Name)
                .WriteNewLine()
                .WriteIndentedLine("{");

            Writer.Indent++;
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
                Writer.WriteNewLine();
            }
        }
    }
}