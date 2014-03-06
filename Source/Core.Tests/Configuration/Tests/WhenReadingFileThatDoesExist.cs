using System.Collections.Generic;
using FluentAssertions;
using SqlFramework.IO;
using Xunit;

namespace SqlFramework.Configuration.Tests
{
    public class WhenReadingFileThatDoesExist : ConfigurationReaderTest
    {
        public WhenReadingFileThatDoesExist()
        {
            SetupExternalBehavior();
            GivenFileExists();
            GivenFileHasContent();
            Act();
        }

        [Fact]
        public void ThenConfigurationShouldContainCorrectStoredProcedures()
        {
            _configuration.StoredProcedures.Should().NotBeNull();
            _configuration.StoredProcedures.Namespace.Should().Be("SpNs");
            _configuration.StoredProcedures.Elements.Count.Should().Be(3);

            _configuration.StoredProcedures.Elements[0].SchemaName.Should().Be("Auth");
            _configuration.StoredProcedures.Elements[0].Name.Should().Be("GetUser");

            _configuration.StoredProcedures.Elements[1].SchemaName.Should().Be("Auth");
            _configuration.StoredProcedures.Elements[1].Name.Should().Be("CreateUser");

            _configuration.StoredProcedures.Elements[2].SchemaName.Should().Be("Core");
            _configuration.StoredProcedures.Elements[2].Name.Should().Be("GetMember");
        }

        [Fact]
        public void ThenConfigurationShouldContainCorrectUserDefinedTableTypes()
        {
            _configuration.UserDefinedTableTypes.Should().NotBeNull();
            _configuration.UserDefinedTableTypes.Namespace.Should().Be("UdtNs");
            _configuration.UserDefinedTableTypes.Elements.Count.Should().Be(2);

            _configuration.UserDefinedTableTypes.Elements[0].SchemaName.Should().Be("Auth");
            _configuration.UserDefinedTableTypes.Elements[0].Name.Should().Be("UserIds");

            _configuration.UserDefinedTableTypes.Elements[1].SchemaName.Should().Be("Core");
            _configuration.UserDefinedTableTypes.Elements[1].Name.Should().Be("MemberIds");
        }

        [Fact]
        public void ThenConfigurationShouldNotBeNull()
        {
            _configuration.Should().NotBeNull();
        }

        [Fact]
        public void ThenTheReadingShouldBeSuccessful()
        {
            _returnValue.Should().BeTrue();
        }

        private void Act()
        {
            _returnValue = ConfigurationReader.TryRead("file", out _configuration);
        }

        private void GivenFileExists()
        {
            StorageProviderMock.SimulateFileExists("file", true);
        }

        private void GivenFileHasContent()
        {
            StorageProviderMock.SimulateOpenStream("file", ResourceFiles.Configuration.Tests.Resources.Configuration_xml);
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