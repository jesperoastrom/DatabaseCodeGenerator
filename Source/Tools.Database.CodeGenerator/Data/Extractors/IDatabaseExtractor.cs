using Flip.Tools.Database.CodeGenerator.Data.Models;



namespace Flip.Tools.Database.CodeGenerator.Data.Extractors
{

	public interface IDatabaseExtractor
	{

		DatabaseModel Extract(Configuration.DatabaseConfiguration configuration);

	}

}
