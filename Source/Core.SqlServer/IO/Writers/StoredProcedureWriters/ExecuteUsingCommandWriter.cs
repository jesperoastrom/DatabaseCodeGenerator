namespace SqlFramework.IO.Writers.StoredProcedureWriters
{
    using System.Data;
    using System.Globalization;
    using CodeBuilders;
    using Data.Models;

    public abstract class ExecuteUsingCommandWriter : ElementWriterBase
    {
        protected ExecuteUsingCommandWriter(ICodeBuilder builder)
            : base(builder)
        {
        }

        /* Example:
         * parameter = command.Parameters.AddWithValue("Products", parameters.Name.GetDataTable());
         * parameter.SqlDbType = SqlDbType.Structured;
         * 
         * parameter = command.Parameters.AddWithValue("Name", parameters.Name);
         * parameter.SqlDbType = SqlDbType.VarChar;
         * parameter.Size = 50;
         * 
         */
        protected void WriteAddParameters(StoredProcedureModel procedure)
        {
            foreach (var parameter in procedure.Parameters)
            {
                if (parameter.Column.ClrType.IsUserDefined)
                {
                    WriteAddWithUserDefinedValue(parameter);
                }
                else
                {
                    WriteAddWithValue(parameter);
                }

                WriteSqlDbType(parameter.SqlDbType);

                if (parameter.Size.HasValue)
                {
                    WriteSize(parameter.Size.Value);
                }

                if (parameter.Precision.HasValue)
                {
                    WritePrecision(parameter.Precision.Value);
                }

                if (parameter.Scale.HasValue)
                {
                    WriteScale(parameter.Scale.Value);
                }

                if (parameter.IsOutput)
                {
                    WriteOutputDirection();
                }

                Builder.WriteNewLine();
            }
        }

        /* Example: 
         * result.Id = (Guid)command.Parameters["Id"].Value;
         * result.Timestamp = (DateTime)command.Parameters["Timestamp"];
         */
        protected void WriteOutputParameters(StoredProcedureModel procedure)
        {
            foreach (var parameter in procedure.OutputParameters)
            {
                Builder
                    .WriteIndentation()
                    .Write("result.")
                    .Write(parameter.Column.PropertyName)
                    .Write(" = (")
                    .Write(parameter.Column.ClrType.TypeName)
                    .Write(")command.Parameters[\"")
                    .Write(parameter.Column.DatabaseName)
                    .Write("\"].Value;")
                    .WriteNewLine();
            }
        }

        private void WriteAddWithUserDefinedValue(ParameterModel parameter)
        {
            Builder
                .WriteIndentation()
                .Write("parameter = command.Parameters.AddWithValue(\"")
                .Write(parameter.Column.DatabaseName)
                .Write("\", parameters.")
                .Write(parameter.Column.PropertyName)
                .Write(".GetDataTable());")
                .WriteNewLine();
        }

        private void WriteAddWithValue(ParameterModel parameter)
        {
            Builder
                .WriteIndentation()
                .Write("parameter = command.Parameters.AddWithValue(\"")
                .Write(parameter.Column.DatabaseName)
                .Write("\", parameters.")
                .Write(parameter.Column.PropertyName)
                .Write(");")
                .WriteNewLine();
        }

        private void WriteOutputDirection()
        {
            Builder
                .WriteIndentation()
                .Write("parameter.Direction = ParameterDirection.Output;")
                .WriteNewLine();
        }

        private void WritePrecision(int precision)
        {
            Builder
                .WriteIndentation()
                .Write("parameter.Precision = ")
                .Write(precision.ToString(CultureInfo.InvariantCulture))
                .Write(";")
                .WriteNewLine();
        }

        private void WriteScale(int scale)
        {
            Builder
                .WriteIndentation()
                .Write("parameter.Scale = ")
                .Write(scale.ToString(CultureInfo.InvariantCulture))
                .Write(";")
                .WriteNewLine();
        }

        private void WriteSize(int size)
        {
            Builder
                .WriteIndentation()
                .Write("parameter.Size = ")
                .Write(size.ToString(CultureInfo.InvariantCulture))
                .Write(";")
                .WriteNewLine();
        }

        private void WriteSqlDbType(SqlDbType sqlDbType)
        {
            Builder
                .WriteIndentation()
                .Write("parameter.SqlDbType = SqlDbType.")
                .Write(sqlDbType.ToString())
                .Write(";")
                .WriteNewLine();
        }
    }
}