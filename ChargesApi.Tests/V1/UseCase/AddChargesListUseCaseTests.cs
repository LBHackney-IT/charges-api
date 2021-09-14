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
    public class AddChargesListUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IChargesListApiGateway> _mockChargesListGateway;
        private readonly AddChargesListUseCase _addChargesListUseCase;

        public AddChargesListUseCaseTests()
        {
            _mockChargesListGateway = new Mock<IChargesListApiGateway>();
            _addChargesListUseCase = new AddChargesListUseCase(_mockChargesListGateway.Object);
        }
        [Fact]
        public async Task AddChargesListReturnsOk()
        {
            var entities = _fixture.Create<AddChargesListRequest>();

            _mockChargesListGateway.Setup(x => x.AddAsync(It.IsAny<ChargesList>()))
                .Returns(Task.CompletedTask);

            var expectedResult = entities.ToDomain();

            var result = await _addChargesListUseCase.ExecuteAsync(entities)
                .ConfigureAwait(false);

            _mockChargesListGateway.Verify(_ => _.AddAsync(It.IsAny<ChargesList>()), Times.Once);
        }
    }
}
