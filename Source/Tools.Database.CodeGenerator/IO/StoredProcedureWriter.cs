using System;
using Flip.Tools.Database.CodeGenerator.Configuration;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	internal sealed class StoredProcedureWriter
	{

		public StoredProcedureWriter(Writer writer)
		{
			this.writer = writer;
		}



		public void Write(StoredProcedures procedures)
		{
			WriteUsings();
			WriteNamespaceStart(procedures.Namespace);

			foreach (var procedure in procedures.Elements)
			{
				WriteProcedure(procedure);
			}
		}



		private void WriteUsings()
		{
			this.writer.WriteLine("using System;");
			this.writer.WriteLine("");
			this.writer.WriteLine("");
			this.writer.WriteLine("");
		}

		private void WriteNamespaceStart(string ns)
		{
			if (string.IsNullOrEmpty(ns))
			{
				throw new ArgumentException("Namespace may not be empty");
			}

			this.writer.Write("namespace ");
			this.writer.Write(ns);
			this.writer.WriteLine(" {");
			this.writer.WriteLine("");
			this.writer.Indent++;
		}

		private void WriteNamespaceEnd()
		{
			this.writer.Indent--;
			this.writer.WriteLine("");
			this.writer.WriteLine("}");
		}

		private void WriteProcedure(StoredProcedureElement procedure)
		{
			//TODO
		}



		private readonly Writer writer;

	}

}
