using ChargeApi.V1.Infrastructure;
using FluentAssertions;
using System;
using Xunit;

namespace ChargeApi.Tests.V1.Infrastructure
{
    public class ExceptionExtensionsTests
    {
        [Fact]
        public void GetFullMessageWithInnerExceptionReturnsFullMessage()
        {
            var exception = new Exception("Test exception", new Exception("Inner Exception 1", new Exception("Inner Exception 2")));

            var expectedResult = "Test exception; Inner Exception 1; Inner Exception 2; ";

            var result = exception.GetFullMessage();

            result.Should().NotBeNull();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetFullMessageWithoutInnerExceptionReturnsMessageFromException()
        {
            var exception = new Exception("Test exception");

            var expectedResult = "Test exception; ";

            var result = exception.GetFullMessage();

            result.Should().NotBeNull();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetFullMessageExceptionNullReturnsStringWithSemicolon()
        {
            var exception = (Exception) null;

            var expectedResult = "Exception message is empty";

            var result = exception.GetFullMessage();

            result.Should().NotBeNull();

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}