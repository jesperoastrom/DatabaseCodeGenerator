namespace SqlFramework.IO
{
    using System.IO;
    using Moq;
    using StorageProviders;

    internal static class StorageProviderMockExtensions
    {
        public static void SimulateFileExists(this Mock<IStorageProvider> mock, string fileName, bool returnValue)
        {
            mock.Setup(x => x.FileExists(fileName)).Returns(returnValue);
        }

        public static void SimulateOpenStream(this Mock<IStorageProvider> mock, string fileName, string resourceName)
        {
            Stream stream = EmbeddedResourceHelper.GetStreamFromEmbeddedResource<TestResources>(resourceName);
            mock.Setup(x => x.OpenStream(fileName)).Returns(stream);
        }
    }
}