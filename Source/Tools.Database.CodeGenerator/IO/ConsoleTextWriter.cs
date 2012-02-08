namespace Flip.Tools.Database.CodeGenerator.IO
{

	public sealed class ConsoleTextWriter : ITextWriter
	{

		public void Write(string s)
		{
			System.Console.Write(s);
		}

		public void WriteLine(string s)
		{
			System.Console.WriteLine(s);
		}

	}

}
