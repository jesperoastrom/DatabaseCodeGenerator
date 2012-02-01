using Flip.Tools.Database.CodeGenerator.Configuration;



namespace Flip.Tools.CodeGenerator.Data
{

	public sealed class Generator
	{

		public Generator(DatabaseConfiguration configuration)
		{
			this.configuration = configuration;
		}



		//public Extract(string outputFile)



		private readonly DatabaseConfiguration configuration;

	}

}
