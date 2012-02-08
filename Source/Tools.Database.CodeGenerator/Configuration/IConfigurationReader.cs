namespace Flip.Tools.Database.CodeGenerator.Configuration
{

	public interface IConfigurationReader
	{

		bool TryRead(string file, out DatabaseConfiguration configuration);

	}

}
