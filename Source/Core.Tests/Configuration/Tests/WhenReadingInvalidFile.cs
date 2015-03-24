namespace SqlFramework.Configuration.Tests
{
    using System.Xml.Schema;
    using IO;
    using Xunit;

    public class WhenReadingInvalidFile : ConfigurationReaderTest
    {
        public WhenReadingInvalidFile()
        {
            GivenFileExists();
            GivenShortestNamespacesCanBeCalculated();
            GivenFileHasInvalidContent();
        }

        [Fact]
        public void ThenExceptionShouldBeThrown()
        {
            Assert.Throws<XmlSchemaException>(() => Act());
        }

        private void Act()
        {
            ConfigurationReader.Read("file");
        }

        private void GivenFileExists()
        {
            StorageProviderMock.SimulateFileExists("file", true);
        }

        private void GivenFileHasInvalidContent()
        {
            StorageProviderMock.SimulateOpenStream("file", Properties.Resources.InvalidDatabaseConfiguration);
        }

        private void GivenShortestNamespacesCanBeCalculated()
        {
            DatabaseToCodeNameConverter.SimulateGetShortestNamespaceTo("SpNs", "UdtNs", "SpNs");
        }
    }
}