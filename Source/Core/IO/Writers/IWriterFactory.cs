using SqlFramework.IO.CodeBuilders;

namespace SqlFramework.IO.Writers
{
    public interface IWriterFactory
    {
        IStoredProcedureWriter CreateStoredProcedureWriter(ICodeBuilder builder);
        IUserDefinedTableTypeWriter CreateUserDefinedTableTypeWriter(ICodeBuilder builder);
        IUsingsWriter CreateUsingsWriter(ICodeBuilder builder);
    }
}
