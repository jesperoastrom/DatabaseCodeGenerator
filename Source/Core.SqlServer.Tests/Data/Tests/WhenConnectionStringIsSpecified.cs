namespace SqlFramework.Data.Tests
{
    using FluentAssertions;
    using Xunit;

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