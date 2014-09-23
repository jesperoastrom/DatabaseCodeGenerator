namespace SqlFramework.IO.Writers.StoredProcedureWriters
{
    using System.Collections.Generic;
    using System.Linq;
    using CodeBuilders;
    using Data.Models;

    public sealed class SqlServerStoredProcedureCollectionWriter : ElementWriterBase, IStoredProcedureWriter
    {
        public SqlServerStoredProcedureCollectionWriter(
            ICodeBuilder builder,
            SqlServerStoredProcedureWriter storedProcedureWriter)
            : base(builder)
        {
            _storedProcedureWriter = storedProcedureWriter;
        }

        public void Write(SchemaCollection<StoredProcedureModel> storedProcedures)
        {
            WriteNamespaceStart(storedProcedures.ElementNamespace);

            foreach (var schema in storedProcedures.SchemaElementCollections.OrderBy(s => s.SchemaName))
            {
                BeginWriteStaticClass(schema.SchemaName);
                {
                    List<StoredProcedureModel> elements = schema.Elements.OrderBy(s => s.DatabaseName).ToList();
                    int lastIndex = elements.Count - 1;

                    for (int i = 0; i < elements.Count; i++)
                    {
                        var element = elements[i];
                        _storedProcedureWriter.Write(element, i == lastIndex);
                    }

                    WriteGetValueOrDefaultMethod();
                }
                WriteBlockEnd();
            }
            WriteBlockEnd();
        }

        private void WriteGetValueOrDefaultMethod()
        {
            Builder.WriteNewLine();
            Builder.WriteIndentedLine("private static T GetValueOrDefault<T>(SqlDataReader reader, string columnName)");
            WriteBlockStart();
            {
                Builder.WriteIndentedLine("return reader.IsDBNull(reader.GetOrdinal(columnName)) ?");
                Builder.Indent++;
                {
                    Builder.WriteIndentedLine("default(T) :");
                    Builder.WriteIndentedLine("(T)reader[columnName];");
                }
                Builder.Indent--; //No block end here
            }
            WriteBlockEnd();
        }

        private readonly SqlServerStoredProcedureWriter _storedProcedureWriter;
    }
}