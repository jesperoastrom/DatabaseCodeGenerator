namespace SqlFramework.Data.Extractors.SqlServer2012.SqlServerCSharpCodeNameConverterTests
{
    using System;
    using FluentAssertions;
    using SqlServer;
    using Xunit;

    public class ToParameterNameTests
    {
        public ToParameterNameTests()
        {
            _converter = new SqlServerCSharpCodeNameConverter();
        }

        [Fact]
        public void WhenNameAlreadyHasParameterFormattingThenParameterNameIsCorrect()
        {
            _converter.ToParameterName("orderId").Should().Be("orderId");
        }

        [Fact]
        public void WhenNameDoesNotHaveParameterFormattingThenParameterNameIsCorrect()
        {
            _converter.ToParameterName("@orderId").Should().Be("orderId");
        }

        [Fact]
        public void WhenNameIsEmptyThenParameterNameThrows()
        {
            Assert.Throws<ArgumentException>(() => _converter.ToParameterName(string.Empty));
        }

        [Fact]
        public void WhenNameIsNullThenParameterNameThrows()
        {
            Assert.Throws<ArgumentNullException>(() => _converter.ToParameterName(null));
        }

        private readonly SqlServerCSharpCodeNameConverter _converter;
    }
}