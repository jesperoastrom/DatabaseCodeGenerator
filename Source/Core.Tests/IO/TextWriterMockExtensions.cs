using System;
using Moq;

namespace SqlFramework.IO
{
    internal static class TextWriterMockExtensions
    {
        public static void SimulateWriteLine(this Mock<ITextWriter> mock, Action<string> action)
        {
            mock.Setup(x => x.WriteLine(It.IsAny<string>())).Callback(action);
        }
    }
}