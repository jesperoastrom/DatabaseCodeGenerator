namespace SqlFramework.Configuration.Tests
{
    using FluentAssertions;
    using IO;
    using Xunit;

    public class WhenReadingFileThatDoesExist : ConfigurationReaderTest
    {
        public WhenReadingFileThatDoesExist()
        {
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

        private void Act()
        {
            _configuration = ConfigurationReader.Read("file");
        }

        private void GivenFileExists()
        {
            StorageProviderMock.SimulateFileExists("file", true);
        }

        private void GivenFileHasContent()
        {
            StorageProviderMock.SimulateOpenStream("file", TestResources.Configuration.Tests.Resources.DatabaseConfiguration_xml);
        }

        private DatabaseConfiguration _configuration;
    }
}