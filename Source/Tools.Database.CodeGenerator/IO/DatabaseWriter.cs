using System;
using System.IO;
using Flip.Tools.Database.CodeGenerator.Configuration;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	public sealed class DatabaseWriter
	{

		public DatabaseWriter(DatabaseConfiguration configuration)
		{
			this.configuration = configuration;
		}



		public void WriteFile(string outputFile, string indentation)
		{
			string directory = Path.GetDirectoryName(outputFile);

			if (!Directory.Exists(directory))
			{
				throw new ArgumentException("Directory '" + directory + "' does not exist");
			}

			using (FileStream stream = File.Open(outputFile, FileMode.Create))
			{
				var writer = new Writer(stream, indentation);
				
				StoredProcedureWriter procedureWriter = new StoredProcedureWriter(writer);
				procedureWriter.Write(this.configuration.StoredProcedures);
			}

		}



		private readonly DatabaseConfiguration configuration;

	}



}
