namespace SqlFramework.IO
{

	public interface IDatabaseWriter
	{

		bool WriteOutput(string configurationFile, string outputFile, string indentation);

	}

}
