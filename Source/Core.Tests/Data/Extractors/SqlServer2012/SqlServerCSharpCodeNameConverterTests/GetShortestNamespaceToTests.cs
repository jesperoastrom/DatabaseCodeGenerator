using System;
using FluentAssertions;
using SqlFramework.Data.SqlServer2012;
using Xunit;

namespace SqlFramework.Data.Extractors.SqlServer2012.SqlServerCSharpCodeNameConverterTests
{
    public class GetShortestNamespaceToTests
    {
        public GetShortestNamespaceToTests()
        {
            _converter = new SqlServerCSharpCodeNameConverter();
        }

        [Fact]
        public void WhenFromDoesNotMatchToThenShortestNamespaceIsTo()
        {
            _converter.GetShortestNamespaceTo("company.product.namespace", "othercompany.product.namespace").Should().Be("othercompany.product.namespace");
        }

        [Fact]
        public void WhenFromIsEmptThenShortestNamespaceThrows()
        {
            Assert.Throws<ArgumentException>(() => _converter.GetShortestNamespaceTo("", "any"));
        }

        [Fact]
        public void WhenFromIsNullThenShortestNamespaceThrows()
        {
            Assert.Throws<ArgumentNullException>(() => _converter.GetShortestNamespaceTo(null, "any"));
        }

        [Fact]
        public void WhenFromMatchesToThenShortestNamespaceIsEmpty()
        {
            _converter.GetShortestNamespaceTo("company.product.namespace", "company.product.namespace").Should().Be("");
        }

        [Fact]
        public void WhenFromPartiallyMatchesToThenShortestNamespaceIsDifference()
        {
            _converter.GetShortestNamespaceTo("company.product.namespace", "company.order.namespace").Should().Be("order.namespace");
        }

        [Fact]
        public void WhenToIsEmptyThenShortestNamespaceIsFrom()
        {
            Assert.Throws<ArgumentException>(() => _converter.GetShortestNamespaceTo("from", ""));
        }

        [Fact]
        public void WhenToIsNullShortestNamespaceThrows()
        {
            Assert.Throws<ArgumentNullException>(() => _converter.GetShortestNamespaceTo("from", null));
        }

        private readonly SqlServerCSharpCodeNameConverter _converter;
    }
}