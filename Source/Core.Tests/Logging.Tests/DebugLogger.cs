using System;
using System.Diagnostics;



namespace SqlFramework.Logging.Tests
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
