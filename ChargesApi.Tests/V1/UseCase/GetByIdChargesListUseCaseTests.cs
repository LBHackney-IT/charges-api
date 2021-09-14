using AutoFixture;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.UseCase
{
    public class GetByIdChargesListUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IChargesListApiGateway> _mockChargesListApiGateway;
        private readonly GetByIdChargesListUseCase _getByIdChargesListUseCase;

        public GetByIdChargesListUseCaseTests()
        {
            _mockChargesListApiGateway = new Mock<IChargesListApiGateway>();
            _getByIdChargesListUseCase = new GetByIdChargesListUseCase(_mockChargesListApiGateway.Object);
        }
        [Fact]
        public async Task GetByIdValidIdReturnsCharge()
        {
            var domain = _fixture.Create<ChargesList>();
            
            var expectedResult = domain.ToResponse();

            _mockChargesListApiGateway.Setup(_ => _.GetChargesListByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(domain);

            var result = await _getByIdChargesListUseCase.ExecuteAsync(Guid.NewGuid())
                .ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetByIdInvalidIdReturnsCharge()
        {
            _mockChargesListApiGateway.Setup(_ => _.GetChargesListByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ChargesList) null);

            var result = await _getByIdChargesListUseCase.ExecuteAsync(Guid.NewGuid())
                .ConfigureAwait(false);

            result.Should().BeNull();
        }
    }
}
