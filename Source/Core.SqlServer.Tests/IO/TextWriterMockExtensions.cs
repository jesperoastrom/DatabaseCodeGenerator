namespace SqlFramework.IO
{
    using System;
    using Moq;
    using OutputDestinations;

    internal static class TextWriterMockExtensions
    {
        public static void SimulateWriteLine(this Mock<IOutputDestination> mock, Action<string> action)
        {
            mock.Setup(x => x.WriteLine(It.IsAny<string>())).Callback(action);
        }
    }
}