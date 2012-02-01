using Flip.Tools.Database.CodeGenerator.Configuration;



namespace Flip.Tools.CodeGenerator.Data
{

	internal sealed class Generator
	{

		public Generator(DatabaseConfiguration configuration)
		{
			this.configuration = configuration;
		}



		//public Generate(string outputFile)



		private readonly DatabaseConfiguration configuration;

	}

}
