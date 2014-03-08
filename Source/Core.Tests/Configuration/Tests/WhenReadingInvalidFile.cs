using System.Xml.Schema;
using SqlFramework.IO;
using Xunit;

namespace SqlFramework.Configuration.Tests
{
    public class WhenReadingInvalidFile : ConfigurationReaderTest
    {
        public WhenReadingInvalidFile()
        {
            GivenFileExists();
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
            StorageProviderMock.SimulateOpenStream("file", TestResources.Configuration.Tests.Resources.InvalidDatabaseConfiguration_xml);
        }
    }
}