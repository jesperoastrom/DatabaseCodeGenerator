namespace SqlFramework.Data.Extractors.SqlServer2012.SqlServerCSharpCodeNameConverterTests
{
    using System;
    using FluentAssertions;
    using SqlServer;
    using Xunit;

    public class EscapeDatabaseNameTests
    {
        public EscapeDatabaseNameTests()
        {
            _converter = new SqlServerCSharpCodeNameConverter();
        }

        [Fact]
        public void WhenNameIsEmptyThenEscapedDatabaseNameThrows()
        {
            Assert.Throws<ArgumentException>(() => _converter.EscapeDatabaseName(string.Empty));
        }

        [Fact]
        public void WhenNameIsEscapedItShouldNotBeChanged()
        {
            _converter.EscapeDatabaseName("[Name]").Should().Be("[Name]");
        }

        [Fact]
        public void WhenNameIsNotEscapedItShouldBeEscaped()
        {
            _converter.EscapeDatabaseName("Name").Should().Be("[Name]");
        }

        [Fact]
        public void WhenNameIsNullThenEscapedDatabaseNameThrows()
        {
            Assert.Throws<ArgumentNullException>(() => _converter.EscapeDatabaseName((string)null));
        }

        [Fact]
        public void WhenNameIsPartiallyEscapedItShouldBeEscaped()
        {
            _converter.EscapeDatabaseName("[Name").Should().Be("[Name]");
            _converter.EscapeDatabaseName("Name]").Should().Be("[Name]");
        }

        private readonly SqlServerCSharpCodeNameConverter _converter;
    }
}