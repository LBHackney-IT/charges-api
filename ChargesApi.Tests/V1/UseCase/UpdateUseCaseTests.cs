using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.UseCase
{
    public class UpdateUseCaseTests
    {
        private readonly Mock<IChargesApiGateway> _mockChargeGateway;
        private readonly UpdateUseCase _updateUseCase;

        public UpdateUseCaseTests()
        {
            _mockChargeGateway = new Mock<IChargesApiGateway>();
            _updateUseCase = new UpdateUseCase(_mockChargeGateway.Object);
        }

        [Fact]
        public async Task UpdateValidModel()
        {
            _mockChargeGateway.Setup(_ => _.UpdateAsync(It.IsAny<Charge>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await _updateUseCase.ExecuteAsync(new ChargeResponse(), string.Empty)
                .ConfigureAwait(false);

            _mockChargeGateway.Verify(_ => _.UpdateAsync(It.IsAny<Charge>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateInvalidModel()
        {
            _mockChargeGateway.Setup(_ => _.AddAsync(It.IsAny<Charge>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            try
            {
                await _updateUseCase.ExecuteAsync(null, string.Empty)
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
