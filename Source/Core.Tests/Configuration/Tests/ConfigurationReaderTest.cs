using Moq;
using SqlFramework.IO;
using SqlFramework.IO.StorageProviders;

namespace SqlFramework.Configuration.Tests
{
    public abstract class ConfigurationReaderTest
    {
        protected ConfigurationReaderTest()
        {
            StorageProviderMock = new Mock<IStorageProvider>(MockBehavior.Strict);
            DatabaseToCodeNameConverter = new Mock<IDatabaseToCodeNameConverter>(MockBehavior.Strict);
            ConfigurationReader = new ConfigurationReader(StorageProviderMock.Object, DatabaseToCodeNameConverter.Object);
        }

        protected readonly ConfigurationReader ConfigurationReader;
        protected readonly Mock<IStorageProvider> StorageProviderMock;
        protected readonly Mock<IDatabaseToCodeNameConverter> DatabaseToCodeNameConverter;
    }
}