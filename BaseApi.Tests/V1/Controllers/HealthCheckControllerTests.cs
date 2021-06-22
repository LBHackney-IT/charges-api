using System.Collections.Generic;
using ChargeApi.V1.Controllers;
using ChargeApi.V1.UseCase;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ChargeApi.Tests.V1.Controllers
{

    
    public class HealthCheckControllerTests
    {
        private HealthCheckController _classUnderTest;


        public HealthCheckControllerTests()
        {
            _classUnderTest = new HealthCheckController();
        }

        [Fact]
        public void ReturnsResponseWithStatus()
        {
            var expected = new Dictionary<string, object> { { "success", true } };
            var response = _classUnderTest.HealthCheck() as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(expected);
        }

        /*
        [Fact]
        public void ThrowErrorThrows()
        {
            Assert.Throws<TestOpsErrorException>(_classUnderTest.ThrowError);
        }
        */
    }
}
