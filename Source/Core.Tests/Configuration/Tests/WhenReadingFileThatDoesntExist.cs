namespace SqlFramework.Configuration.Tests
{
    using System.IO;
    using IO;
    using Xunit;

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