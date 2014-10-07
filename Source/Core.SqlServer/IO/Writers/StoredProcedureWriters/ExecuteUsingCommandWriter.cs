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
         * p = c.Parameters.AddWithValue("Products", parameters.Name.GetDataTable());
         * p.SqlDbType = SqlDbType.Structured;
         * 
         * p = c.Parameters.AddWithValue("Name", parameters.Name);
         * p.SqlDbType = SqlDbType.VarChar;
         * p.Size = 50;
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
         * r.Id = (Guid)c.Parameters["Id"].Value;
         * r.Timestamp = (DateTime)c.Parameters["Timestamp"];
         */
        protected void WriteOutputParameters(StoredProcedureModel procedure)
        {
            foreach (var parameter in procedure.OutputParameters)
            {
                Builder
                    .WriteIndentation()
                    .Write("r.")
                    .Write(parameter.Column.PropertyName)
                    .Write(" = (")
                    .Write(parameter.Column.ClrType.TypeName)
                    .Write(")c.Parameters[\"")
                    .Write(parameter.Column.DatabaseName)
                    .Write("\"].Value;")
                    .WriteNewLine();
            }
        }

        private void WriteAddWithUserDefinedValue(ParameterModel parameter)
        {
            Builder
                .WriteIndentation()
                .Write("p = c.Parameters.AddWithValue(\"")
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
                .Write("p = c.Parameters.AddWithValue(\"")
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
                .Write("p.Direction = ParameterDirection.Output;")
                .WriteNewLine();
        }

        private void WritePrecision(int precision)
        {
            Builder
                .WriteIndentation()
                .Write("p.Precision = ")
                .Write(precision.ToString(CultureInfo.InvariantCulture))
                .Write(";")
                .WriteNewLine();
        }

        private void WriteScale(int scale)
        {
            Builder
                .WriteIndentation()
                .Write("p.Scale = ")
                .Write(scale.ToString(CultureInfo.InvariantCulture))
                .Write(";")
                .WriteNewLine();
        }

        private void WriteSize(int size)
        {
            Builder
                .WriteIndentation()
                .Write("p.Size = ")
                .Write(size.ToString(CultureInfo.InvariantCulture))
                .Write(";")
                .WriteNewLine();
        }

        private void WriteSqlDbType(SqlDbType sqlDbType)
        {
            Builder
                .WriteIndentation()
                .Write("p.SqlDbType = SqlDbType.")
                .Write(sqlDbType.ToString())
                .Write(";")
                .WriteNewLine();
        }
    }
}