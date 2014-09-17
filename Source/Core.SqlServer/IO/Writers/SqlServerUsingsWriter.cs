namespace SqlFramework.IO.Writers
{
    using CodeBuilders;

    public class SqlServerUsingsWriter : ElementWriterBase, IUsingsWriter
    {
        public SqlServerUsingsWriter(ICodeBuilder builder) : base(builder)
        {
        }

        public void WriteUsings()
        {
            Builder
                .WriteIndentedLine("using System;")
                .WriteIndentedLine("using System.Collections.Generic;")
                .WriteIndentedLine("using System.Data;")
                .WriteIndentedLine("using System.Data.SqlClient;")
                .WriteNewLine();
        }
    }
}