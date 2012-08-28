using System;
using System.Diagnostics;



namespace Flip.Tools.Database.CodeGenerator.Tests.Logging
{

	public sealed class DebugLogger : ILogger
	{

		public void Log(string message)
		{
			this.WriteLogMessage(message);
		}

		[Conditional("DEBUG")]
		private void WriteLogMessage(string message)
		{
			Console.WriteLine(message);
		}

	}

}
