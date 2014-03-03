using Moq;

namespace SqlFramework.IO
{
    internal static class StorageProviderMockExtensions
    {
        public static void SimulateFileExists(this Mock<IStorageProvider> mock, string fileName, bool returnValue)
        {
            mock.Setup(x => x.FileExists(fileName)).Returns(returnValue);
        }
    }
}