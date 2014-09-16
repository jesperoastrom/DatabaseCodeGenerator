using System;
using SqlFramework.IO.CodeBuilders;

namespace SqlFramework.IO.Writers
{
    public abstract class ElementWriterBase
    {
        protected ElementWriterBase(ICodeBuilder builder)
        {
            Builder = builder;
        }

        protected ICodeBuilder Builder { get; private set; }

        protected void BeginWriteStaticClass(string className)
        {
            Builder
                .WriteIndentation()
                .Write("public static partial class ")
                .Write(className)
                .WriteNewLine()
                .WriteIndentedLine("{");

            Builder.Indent++;
        }

        protected void WriteBlockEnd()
        {
            Builder.Indent--;
            Builder.WriteIndentedLine("}");
        }

        protected void WriteNamespaceStart(string ns)
        {
            if (string.IsNullOrEmpty(ns))
            {
                throw new ArgumentException("Namespace may not be empty");
            }

            Builder
                .Write("namespace ")
                .Write(ns)
                .WriteNewLine()
                .WriteIndentedLine("{");

            Builder.Indent++;
        }
    }
}