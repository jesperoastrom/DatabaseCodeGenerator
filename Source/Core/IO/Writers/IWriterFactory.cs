namespace SqlFramework.IO.Writers
{
    using CodeBuilders;

    public interface IWriterFactory
    {
        IStoredProcedureWriter CreateStoredProcedureWriter(ICodeBuilder builder);
        
        IUserDefinedTableTypeWriter CreateUserDefinedTableTypeWriter(ICodeBuilder builder);
        
        IUsingsWriter CreateUsingsWriter(ICodeBuilder builder);
    }
}