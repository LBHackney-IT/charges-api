using AutoFixture;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.UseCase
{
    public class GetAllChargesListUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IChargesListApiGateway> _mockChargesListGateway;
        private readonly GetAllChargesListUseCase _getAllChargesListUseCase;
        public GetAllChargesListUseCaseTests()
        {
            _mockChargesListGateway = new Mock<IChargesListApiGateway>();
            _getAllChargesListUseCase = new GetAllChargesListUseCase(_mockChargesListGateway.Object);
        }
        [Fact]
        public async Task GetsAllChargeTypeEstate()
        {
            var entities = _fixture.Create<List<ChargesList>>();

            _mockChargesListGateway.Setup(x => x.GetAllChargesListAsync(It.IsAny<string>()))
                .ReturnsAsync(entities);

            var expectedResult = entities.ToResponse();

            var result = await _getAllChargesListUseCase.ExecuteAsync("DCB").ConfigureAwait(false);

            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(entities.Count);

            result.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task GetsAllChargeTypeBlock()
        {
            var entities = _fixture.Create<List<ChargesList>>();

            _mockChargesListGateway.Setup(x => x.GetAllChargesListAsync(It.IsAny<string>()))
                .ReturnsAsync(entities);

            var expectedResult = entities.ToResponse();

            var result = await _getAllChargesListUseCase.ExecuteAsync("DCB").ConfigureAwait(false);

            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
