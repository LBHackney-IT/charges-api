using ChargeApi.V1.Boundary.Request;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ChargeApi.Tests.V1.UseCase
{
    public class AddUseCaseTests
    {
        private readonly Mock<IChargeApiGateway> _mockChargeGateway;
        private readonly AddUseCase _addUseCase;

        public AddUseCaseTests()
        {
            _mockChargeGateway = new Mock<IChargeApiGateway>();
            _addUseCase = new AddUseCase(_mockChargeGateway.Object);
        }

        [Fact]
        public async Task AddValidModel()
        {
            AddChargeRequest charge = new AddChargeRequest();

            _mockChargeGateway.Setup(_ => _.AddAsync(It.IsAny<Charge>()))
                .Returns(Task.CompletedTask);

            await _addUseCase.ExecuteAsync(charge).ConfigureAwait(false);

            _mockChargeGateway.Verify(_ => _.AddAsync(It.IsAny<Charge>()), Times.Once);
        }

        [Fact]
        public async Task AddInvalidModel()
        {
            _mockChargeGateway.Setup(_ => _.AddAsync(It.IsAny<Charge>()))
                .Returns(Task.CompletedTask);

            try
            {
                await _addUseCase.ExecuteAsync(null)
                    .ConfigureAwait(false);

                Assert.True(false, "ArgumentNullException should be thrown!");
            }
            catch (Exception ex)
            {
                ex.Should().NotBeNull();
                ex.Should().BeOfType<ArgumentNullException>();
                ex.Message.Should().Be("Value cannot be null. (Parameter 'charge')");
            }
        }
    }
}
