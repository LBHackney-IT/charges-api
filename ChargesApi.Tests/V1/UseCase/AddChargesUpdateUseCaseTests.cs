using AutoFixture;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.UseCase
{
    public class AddChargesUpdateUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ISnsGateway> _mockSnsGateway;
        private readonly Mock<ISnsFactory> _mockSnsFactory;
        private readonly AddChargesUpdateUseCase _addChargesUpdateUseCase;

        public AddChargesUpdateUseCaseTests()
        {
            _mockSnsGateway = new Mock<ISnsGateway>();
            _mockSnsFactory = new Mock<ISnsFactory>();
            _addChargesUpdateUseCase = new AddChargesUpdateUseCase(_mockSnsGateway.Object, _mockSnsFactory.Object);
        }
        [Fact]
        public async Task AddChargesUpdateReturnsOk()
        {
            var entities = _fixture.Create<ChargesSns>();
            var request = _fixture.Create<AddChargesUpdateRequest>();
            _mockSnsFactory.Setup(x => x.Create(It.IsAny<AddChargesUpdateRequest>()))
                .Returns(entities);

            _mockSnsGateway.Setup(x => x.Publish(entities))
                .Returns(Task.CompletedTask);

            var result = await _addChargesUpdateUseCase.ExecuteAsync(request)
                .ConfigureAwait(false);

            _mockSnsGateway.Verify(_ => _.Publish(It.IsAny<ChargesSns>()), Times.Once);
            _mockSnsFactory.Verify(_ => _.Create(It.IsAny<AddChargesUpdateRequest>()), Times.Once);
            result.IsSuccess.Should().Be(true);
        }
    }
}
