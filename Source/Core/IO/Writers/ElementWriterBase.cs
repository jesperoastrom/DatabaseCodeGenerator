namespace SqlFramework.IO.Writers
{
    using System;
    using CodeBuilders;

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
                .WriteNewLine();

            WriteBlockStart();
        }

        protected void WriteBlockStart()
        {
            Builder.Indent++;
            Builder.WriteIndentedLine("{");
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
                .WriteNewLine();

            WriteBlockStart();
        }
    }
}