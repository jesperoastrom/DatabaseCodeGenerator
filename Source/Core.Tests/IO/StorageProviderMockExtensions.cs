using System.IO;
using Moq;

namespace SqlFramework.IO
{
    internal static class StorageProviderMockExtensions
    {
        public static void SimulateFileExists(this Mock<IStorageProvider> mock, string fileName, bool returnValue)
        {
            mock.Setup(x => x.FileExists(fileName)).Returns(returnValue);
        }

        public static void SimulateOpenStream(this Mock<IStorageProvider> mock, string fileName, string resourceName)
        {
            Stream stream = EmbeddedResourceHelper.GetStreamFromEmbeddedResource(resourceName);
            mock.Setup(x => x.OpenStream(fileName)).Returns(stream);
        }
    }
}