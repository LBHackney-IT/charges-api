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
    public class UpdateUseCaseTests
    {
        private readonly Mock<IChargeApiGateway> _mockChargeGateway;
        private readonly UpdateUseCase _updateUseCase;

        public UpdateUseCaseTests()
        {
            _mockChargeGateway = new Mock<IChargeApiGateway>();
            _updateUseCase = new UpdateUseCase(_mockChargeGateway.Object);
        }

        [Fact]
        public async Task UpdateValidModel()
        {
            _mockChargeGateway.Setup(_ => _.UpdateAsync(It.IsAny<Charge>()))
                .Returns(Task.CompletedTask);

            await _updateUseCase.ExecuteAsync(new UpdateChargeRequest())
                .ConfigureAwait(false);

            _mockChargeGateway.Verify(_ => _.UpdateAsync(It.IsAny<Charge>()), Times.Once);
        }

        [Fact]
        public async Task UpdateInvalidModel()
        {
            _mockChargeGateway.Setup(_ => _.AddAsync(It.IsAny<Charge>()))
                .Returns(Task.CompletedTask);

            try
            {
                await _updateUseCase.ExecuteAsync(null)
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
