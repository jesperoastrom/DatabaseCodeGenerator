namespace SqlFramework.IO
{
    public interface IWriterFactory
    {
        IStoredProcedureWriter CreateStoredProcedureWriter(ICodeWriter writer);
        IUserDefinedTableTypeWriter CreateUserDefinedTableTypeWriter(ICodeWriter writer);
        IUsingsWriter CreateUsingsWriter(ICodeWriter writer);
    }
}
