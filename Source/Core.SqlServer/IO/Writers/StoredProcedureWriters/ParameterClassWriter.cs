/* Example:
 * public partial class Parameters
 * {
 *    public Parameters(int number, string name)
 *    {
 *        this.Number = number;
 *        this.Name = name;
 *    }
 *    
 *    public int Number { get; private set; }
 *    
 *    public string Name { get; private set; }
 * }
 */

namespace SqlFramework.IO.Writers.StoredProcedureWriters
{
    using CodeBuilders;
    using Data.Models;

    public sealed class ParameterClassWriter : ElementWriterBase
    {
        public ParameterClassWriter(ICodeBuilder builder) : base(builder)
        {
        }

        public void Write(StoredProcedureModel procedure)
        {
            if (procedure.Parameters.Count > 0)
            {
                Builder
                    .WriteNewLine()
                    .WriteIndentedLine("public partial class Parameters");

                WriteBlockStart();
                {
                    WriteConstructor(procedure);
                    Builder.WriteNewLine();
                    WriteProperties(procedure);
                }
                WriteBlockEnd();
            }
        }

        private void WriteConstructor(StoredProcedureModel procedure)
        {
            Builder
                .WriteIndentation()
                .Write("public Parameters(");

            WriteConstructorArguments(procedure);

            Builder
                .Write(")")
                .WriteNewLine();

            WriteBlockStart();
            {
                WriteConstructorBody(procedure);
            }
            WriteBlockEnd();
        }

        private void WriteConstructorArguments(StoredProcedureModel procedure)
        {
            int lastIndex = procedure.Parameters.Count - 1;
            for (int i = 0; i < procedure.Parameters.Count; i++)
            {
                ParameterModel parameter = procedure.Parameters[i];

                Builder
                    .Write(parameter.Column.ClrType.TypeName)
                    .Write(" ")
                    .Write(parameter.Column.ParameterName);

                if (i != lastIndex)
                {
                    Builder.Write(", ");
                }
            }
        }
        
        private void WriteConstructorBody(StoredProcedureModel procedure)
        {
            foreach (var parameter in procedure.Parameters)
            {
                Builder
                    .WriteIndentation()
                    .Write("this.")
                    .Write(parameter.Column.PropertyName)
                    .Write(" = ")
                    .Write(parameter.Column.ParameterName)
                    .Write(";")
                    .WriteNewLine();
            }
        }

        private void WriteProperties(StoredProcedureModel procedure)
        {
            int lastIndex = procedure.Parameters.Count - 1;
            for (int i = 0; i < procedure.Parameters.Count; i++)
            {
                ParameterModel parameter = procedure.Parameters[i];

                Builder
                    .WriteIndentation()
                    .Write("public ")
                    .Write(parameter.Column.ClrType.TypeName)
                    .Write(" ")
                    .Write(parameter.Column.PropertyName)
                    .Write(" { get; private set; }")
                    .WriteNewLine();

                if (i != lastIndex)
                {
                    Builder.WriteNewLine();
                }
            }
        }
    }
}