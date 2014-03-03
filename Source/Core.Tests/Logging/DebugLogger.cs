using System;
using System.Diagnostics;

namespace SqlFramework.Logging
{
    public sealed class DebugLogger : ILogger
    {
        [Conditional("DEBUG")]
        private void WriteLogMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void Log(string message)
        {
            this.WriteLogMessage(message);
        }
    }
}