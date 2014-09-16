using SqlFramework.IO.CodeBuilders;

namespace SqlFramework.IO.Writers
{
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
