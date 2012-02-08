namespace Flip.Tools.Database.CodeGenerator.IO
{

	public interface IDatabaseWriter
	{

		bool WriteOutput(string configurationFile, string outputFile, string indentation);

	}

}
