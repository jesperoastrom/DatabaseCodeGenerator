using System.Collections.Generic;
using System.Linq;
using Flip.Tools.Database.CodeGenerator.Data.Models;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	public sealed class UserDefinedTableTypeWriter : ElementWriterBase
	{

		public UserDefinedTableTypeWriter(ICodeWriter writer)
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



		private void WriteType(UserDefinedTableTypeModel type, bool isLast)
		{
			this.writer
				.WriteIndentation()
				.Write("public partial class ")
				.Write(type.TypeName.Name)
				.WriteNewLine()
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				WriteConstructor();
				WriteAddRowMethod();
				WriteGetDataTableMethod(type);
				WritePrivateMembers();
				WriteRowClass(type);
			}
			WriteBlockEnd();

			if (!isLast)
			{
				this.writer.WriteNewLine();
			}

		}

		private void WriteConstructor()
		{
			this.writer
				.WriteIndentation()
				.Write("public TestTableIds()")
				.WriteNewLine()
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				this.writer
					.WriteIndentedLine("this.rows = new List<Row>();");
			}
			WriteBlockEnd();
		}

		private void WriteAddRowMethod()
		{
			this.writer
				.WriteNewLine()
				.WriteIndentedLine("public void AddRow(Row row)")
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				this.writer
					.WriteIndentedLine("this.rows.Add(row);");
			}
			WriteBlockEnd();
		}

		private void WriteGetDataTableMethod(UserDefinedTableTypeModel type)
		{
			this.writer
				.WriteNewLine()
				.WriteIndentedLine("public DataTable GetDataTable()")
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				WriteGetDataTableMethodBody(type);
			}
			WriteBlockEnd();
		}

		private void WriteGetDataTableMethodBody(UserDefinedTableTypeModel type)
		{
			this.writer
				.WriteIndentedLine("DataTable table = new DataTable();");

			foreach (var column in type.Columns)
			{
				this.writer
					.WriteIndentation()
					.Write("table.Columns.Add(\"")
					.Write(column.DatabaseName)
					.Write("\", typeof(")
					.Write(column.ClrType)
					.Write("));")
					.WriteNewLine();
			}

			this.writer
				.WriteNewLine()
				.WriteIndentedLine("foreach (Row row in this.rows)")
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				this.writer
					.WriteIndentedLine("DataRow dataRow = table.NewRow();");

				foreach (var column in type.Columns)
				{
					this.writer
						.WriteIndentation()
						.Write("dataRow[\"")
						.Write(column.DatabaseName)
						.Write("\"] = row.")
						.Write(column.PropertyName)
						.Write(";")
						.WriteNewLine()
						.WriteIndentedLine("table.Rows.Add(dataRow);");
				}
			}
			WriteBlockEnd();

			this.writer
				.WriteIndentedLine("return table;");
		}

		private void WritePrivateMembers()
		{
			this.writer
				.WriteNewLine()
				.WriteIndentedLine("private readonly List<Row> rows;");
		}

		private void WriteRowClass(UserDefinedTableTypeModel type)
		{
			this.writer
				.WriteNewLine()
				.WriteIndentedLine("public partial class Row")
				.WriteIndentedLine("{");

			this.writer.Indent++;
			{
				WriteRowClassBody(type);
			}
			WriteBlockEnd();
		}

		private void WriteRowClassBody(UserDefinedTableTypeModel type)
		{
			this.writer
				.WriteIndentation()
				.Write("public Row(");

			WriteRowClassConstructorArgumentList(type);

			this.writer
				.Write(")")
				.WriteNewLine()
				.WriteIndentedLine("{");

			this.writer.Indent++;
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

				this.writer
					.Write(column.ClrType)
					.Write(" ")
					.Write(column.ParameterName);

				if (i != lastIndex)
				{
					this.writer
						.Write(", ");
				}
			}
		}

		private void WriteRowClassConstructorBody(UserDefinedTableTypeModel type)
		{
			foreach (var column in type.Columns)
			{
				this.writer
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
				this.writer
					.WriteIndentation()
					.Write("public ")
					.Write(column.ClrType)
					.Write(" ")
					.Write(column.PropertyName)
					.Write(" { get; private set; }")
					.WriteNewLine();
			}
		}

	}

}
