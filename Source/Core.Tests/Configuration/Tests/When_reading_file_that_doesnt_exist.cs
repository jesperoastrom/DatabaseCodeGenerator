using System.Collections.Generic;
using FluentAssertions;
using SqlFramework.IO;
using Xunit;

namespace SqlFramework.Configuration.Tests
{
// ReSharper disable InconsistentNaming
    public class When_reading_file_that_doesnt_exist : ConfigurationReaderTests
// ReSharper restore InconsistentNaming
    {
        public When_reading_file_that_doesnt_exist()
        {
            _output = new List<string>(1);
            _textWriterMock.SimulateWriteLine(s => _output.Add(s));
            _storageProviderMock.SimulateFileExists("nofile", false);
        }

        [Fact]
        public void Then_false_should_be_returned()
        {
            DatabaseConfiguration configuration;
            _configurationReader.TryRead("nofile", out configuration).Should().BeFalse();
            configuration.Should().BeNull();
            _output.Count.Should().Be(1);
        }

        private readonly List<string> _output;
    }
}