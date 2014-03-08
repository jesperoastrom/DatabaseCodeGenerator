using Moq;
using SqlFramework.IO;

namespace SqlFramework.Configuration.Tests
{
    public abstract class ConfigurationReaderTest
    {
        protected ConfigurationReaderTest()
        {
            StorageProviderMock = new Mock<IStorageProvider>(MockBehavior.Strict);
            ConfigurationReader = new ConfigurationReader(StorageProviderMock.Object);
        }

        protected readonly ConfigurationReader ConfigurationReader;
        protected readonly Mock<IStorageProvider> StorageProviderMock;
    }
}