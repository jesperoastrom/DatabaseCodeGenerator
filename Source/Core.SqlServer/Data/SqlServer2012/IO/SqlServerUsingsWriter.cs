using SqlFramework.IO;

namespace SqlFramework.Data.SqlServer2012.IO
{
    public class SqlServerUsingsWriter : ElementWriterBase, IUsingsWriter
    {
        public SqlServerUsingsWriter(ICodeWriter writer) : base(writer)
        {
        }

        public void WriteUsings()
        {
            Writer
                .WriteIndentedLine("using System;")
                .WriteIndentedLine("using System.Collections.Generic;")
                .WriteIndentedLine("using System.Data;")
                .WriteIndentedLine("using System.Data.SqlClient;")
                .WriteNewLine();
        }
    }
}
