using System;



namespace SqlFramework.IO
{

	public abstract class ElementWriterBase
	{

		protected ElementWriterBase(ICodeWriter writer)
		{
			this.writer = writer;
		}



		protected void WriteNamespaceStart(string ns)
		{
			if (string.IsNullOrEmpty(ns))
			{
				throw new ArgumentException("Namespace may not be empty");
			}

			this.writer
				.Write("namespace ")
				.Write(ns)
				.WriteNewLine()
				.WriteIndentedLine("{");

			this.writer.Indent++;
		}

		protected void WriteBlockEnd()
		{
			this.writer.Indent--;
			this.writer.WriteIndentedLine("}");
		}

		protected void BeginWriteStaticClass(string className)
		{
			this.writer
				.WriteIndentation()
				.Write("public static partial class ")
				.Write(className)
				.WriteNewLine()
				.WriteIndentedLine("{");

			this.writer.Indent++;
		}



		protected readonly ICodeWriter writer;

	}

}
