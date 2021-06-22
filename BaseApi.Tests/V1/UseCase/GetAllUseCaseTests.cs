using System.Linq;
using AutoFixture;
using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChargeApi.Tests.V1.UseCase
{
    public class GetAllUseCaseTests
    {
        /*private Mock<IChargeApiGateway> _mockGateway;
        private GetAllUseCase _classUnderTest;
        private Fixture _fixture;

        public GetAllUseCaseTests()
        {

            _mockGateway = new Mock<IChargeApiGateway>();
            _classUnderTest = new GetAllUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public void GetsAllFromTheGateway()
        {
            var stubbedEntities = _fixture.CreateMany<Entity>().ToList();
            _mockGateway.Setup(x => x.GetAllCharges()).Returns(stubbedEntities);

            var expectedResponse = new ChargeResponseObjectList { ChargeResponseObjects = stubbedEntities.ToResponse() };

            _classUnderTest.Execute().Should().BeEquivalentTo(expectedResponse);
        }*/
    }
}
