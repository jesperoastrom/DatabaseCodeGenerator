using System;
using Moq;
using SqlFramework.IO.OutputDestinations;

namespace SqlFramework.IO
{
    internal static class TextWriterMockExtensions
    {
        public static void SimulateWriteLine(this Mock<IOutputDestination> mock, Action<string> action)
        {
            mock.Setup(x => x.WriteLine(It.IsAny<string>())).Callback(action);
        }
    }
}