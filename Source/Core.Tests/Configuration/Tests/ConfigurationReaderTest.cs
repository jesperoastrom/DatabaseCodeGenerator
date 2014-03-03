using Moq;
using SqlFramework.IO;

namespace SqlFramework.Configuration.Tests
{
    public abstract class ConfigurationReaderTest
    {
        protected ConfigurationReaderTest()
        {
            TextWriterMock = new Mock<ITextWriter>(MockBehavior.Strict);
            StorageProviderMock = new Mock<IStorageProvider>(MockBehavior.Strict);
            ConfigurationReader = new ConfigurationReader(TextWriterMock.Object, StorageProviderMock.Object);
        }

        protected readonly ConfigurationReader ConfigurationReader;
        protected readonly Mock<IStorageProvider> StorageProviderMock;
        protected readonly Mock<ITextWriter> TextWriterMock;
    }
}