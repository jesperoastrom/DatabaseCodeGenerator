using System.Collections.Generic;
using FluentAssertions;
using SqlFramework.IO;
using Xunit;

namespace SqlFramework.Configuration.Tests
{
    public class WhenReadingFileThatDoesntExist : ConfigurationReaderTest
    {
        public WhenReadingFileThatDoesntExist()
        {
            SetupExternalBehavior();
            GivenFileDoesNotExist();
            Act();
        }

        [Fact]
        public void ThenConfigurationShouldBeNull()
        {
            _configuration.Should().BeNull();
        }

        [Fact]
        public void ThenFalseShouldBeReturned()
        {
            _returnValue.Should().BeFalse();
        }

        [Fact]
        public void ThenLogShouldContainError()
        {
            _output.Count.Should().Be(1);
        }

        private void Act()
        {
            _returnValue = ConfigurationReader.TryRead("nofile", out _configuration);
        }

        private void GivenFileDoesNotExist()
        {
            StorageProviderMock.SimulateFileExists("nofile", false);
        }

        private void SetupExternalBehavior()
        {
            _output = new List<string>(1);
            TextWriterMock.SimulateWriteLine(s => _output.Add(s));
        }

        private DatabaseConfiguration _configuration;
        private List<string> _output;
        private bool _returnValue;
    }
}