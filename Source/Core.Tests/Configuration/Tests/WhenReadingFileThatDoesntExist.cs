using System.Collections.Generic;
using FluentAssertions;
using SqlFramework.IO;
using Xunit;

namespace SqlFramework.Configuration.Tests
{
    public class WhenReadingFileThatDoesntExist : ConfigurationReaderTests
    {
        public WhenReadingFileThatDoesntExist()
        {
            _output = new List<string>(1);
            _textWriterMock.SimulateWriteLine(s => _output.Add(s));
            _storageProviderMock.SimulateFileExists("nofile", false);

            _returnValue = _configurationReader.TryRead("nofile", out _configuration);
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

        private readonly DatabaseConfiguration _configuration;
        private readonly List<string> _output;
        private readonly bool _returnValue;
    }
}