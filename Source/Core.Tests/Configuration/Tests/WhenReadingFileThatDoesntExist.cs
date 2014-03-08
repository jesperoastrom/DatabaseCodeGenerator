using System.IO;
using SqlFramework.IO;
using Xunit;

namespace SqlFramework.Configuration.Tests
{
    public class WhenReadingFileThatDoesntExist : ConfigurationReaderTest
    {
        public WhenReadingFileThatDoesntExist()
        {
            GivenFileDoesNotExist();
        }

        [Fact]
        public void ThenExceptionShouldBeThrown()
        {
            Assert.Throws<FileNotFoundException>(() => Act());
        }

        private void Act()
        {
            ConfigurationReader.Read("nofile");
        }

        private void GivenFileDoesNotExist()
        {
            StorageProviderMock.SimulateFileExists("nofile", false);
        }
    }
}