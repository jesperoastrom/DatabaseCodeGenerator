namespace SqlFramework.IO.Writers
{
    public interface IDatabaseWriter
    {
        bool WriteOutput(string configurationFile, string outputFile, string indentation);
    }
}