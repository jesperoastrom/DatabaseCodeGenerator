using FluentAssertions;
using Xunit;

namespace SqlFramework.Tests
{
    public class EscapeDatabaseNameTests
    {
        [Fact]
        public void WhenNameIsEmptyThenEscapedDatabaseNameShouldBeEmpty()
        {
            "".EscapeDatabaseName().Should().Be(string.Empty);
        }

        [Fact]
        public void WhenNameIsEscapedItShouldNotBeChanged()
        {
            "[Name]".EscapeDatabaseName().Should().Be("[Name]");
        }

        [Fact]
        public void WhenNameIsNotEscapedItShouldBeEscaped()
        {
            "Name".EscapeDatabaseName().Should().Be("[Name]");
        }

        [Fact]
        public void WhenNameIsNullThenEscapedDatabaseNameShouldBeNull()
        {
            ((string) null).EscapeDatabaseName().Should().BeNull();
        }

        [Fact]
        public void WhenNameIsPartiallyEscapedItShouldBeEscaped()
        {
            "[Name".EscapeDatabaseName().Should().Be("[Name]");
            "Name]".EscapeDatabaseName().Should().Be("[Name]");
        }
    }
}