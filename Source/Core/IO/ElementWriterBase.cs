using System;

namespace SqlFramework.IO
{
    public abstract class ElementWriterBase
    {
        protected ElementWriterBase(ICodeWriter writer)
        {
            Writer = writer;
        }

        protected ICodeWriter Writer { get; private set; }

        protected void BeginWriteStaticClass(string className)
        {
            Writer
                .WriteIndentation()
                .Write("public static partial class ")
                .Write(className)
                .WriteNewLine()
                .WriteIndentedLine("{");

            Writer.Indent++;
        }

        protected void WriteBlockEnd()
        {
            Writer.Indent--;
            Writer.WriteIndentedLine("}");
        }

        protected void WriteNamespaceStart(string ns)
        {
            if (string.IsNullOrEmpty(ns))
            {
                throw new ArgumentException("Namespace may not be empty");
            }

            Writer
                .Write("namespace ")
                .Write(ns)
                .WriteNewLine()
                .WriteIndentedLine("{");

            Writer.Indent++;
        }
    }
}