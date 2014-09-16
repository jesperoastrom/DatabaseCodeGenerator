using FluentAssertions;
using Xunit;

namespace SqlFramework.Data.Tests
{
    public class WhenConnectionStringIsSpecified
    {
        public WhenConnectionStringIsSpecified()
        {
            ConnectionStringProvider = new ConnectionStringProvider("connection");
        }

        [Fact]
        public void ThenConnectionStringCanBeProvided()
        {
            ConnectionStringProvider.ConnectionString.Should().Be("connection");
        }

        protected readonly ConnectionStringProvider ConnectionStringProvider;
    }
}
