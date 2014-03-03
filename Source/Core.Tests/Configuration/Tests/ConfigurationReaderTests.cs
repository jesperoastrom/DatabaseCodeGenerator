using Moq;
using SqlFramework.IO;

namespace SqlFramework.Configuration.Tests
{
    public abstract class ConfigurationReaderTests
    {
        public ConfigurationReaderTests()
        {
            _textWriterMock = new Mock<ITextWriter>(MockBehavior.Strict);
            _storageProviderMock = new Mock<IStorageProvider>(MockBehavior.Strict);

            _configurationReader = new ConfigurationReader(_textWriterMock.Object, _storageProviderMock.Object);
        }

        protected readonly ConfigurationReader _configurationReader;
        protected readonly Mock<IStorageProvider> _storageProviderMock;
        protected readonly Mock<ITextWriter> _textWriterMock;
    }
}