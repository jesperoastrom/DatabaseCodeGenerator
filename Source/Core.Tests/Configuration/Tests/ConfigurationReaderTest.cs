namespace SqlFramework.Configuration.Tests
{
    using IO.StorageProviders;
    using Moq;

    public abstract class ConfigurationReaderTest
    {
        protected ConfigurationReaderTest()
        {
            StorageProviderMock = new Mock<IStorageProvider>(MockBehavior.Strict);
            DatabaseToCodeNameConverter = new Mock<IDatabaseToCodeNameConverter>(MockBehavior.Strict);
            ConfigurationReader = new ConfigurationReader(StorageProviderMock.Object, DatabaseToCodeNameConverter.Object);
        }

        protected readonly ConfigurationReader ConfigurationReader;
        protected readonly Mock<IDatabaseToCodeNameConverter> DatabaseToCodeNameConverter;
        protected readonly Mock<IStorageProvider> StorageProviderMock;
    }
}