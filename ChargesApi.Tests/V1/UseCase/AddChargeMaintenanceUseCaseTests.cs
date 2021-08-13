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
    public class AddChargeMaintenanceUseCaseTests
    {
        private readonly Mock<IChargeMaintenanceApiGateway> _mockChargeMaintenanceGateway;
        private readonly AddChargeMaintenanceUseCase _addUseCase;

        public AddChargeMaintenanceUseCaseTests()
        {
            _mockChargeMaintenanceGateway = new Mock<IChargeMaintenanceApiGateway>();
            _addUseCase = new AddChargeMaintenanceUseCase(_mockChargeMaintenanceGateway.Object);
        }
        [Fact]
        public async Task AddValidModel()
        {
            var chargeMaintenance = new AddChargeMaintenanceRequest();

            _mockChargeMaintenanceGateway.Setup(_ => _.AddAsync(It.IsAny<ChargeMaintenance>()))
                .Returns(Task.CompletedTask);

            await _addUseCase.ExecuteAsync(chargeMaintenance).ConfigureAwait(false);

            _mockChargeMaintenanceGateway.Verify(_ => _.AddAsync(It.IsAny<ChargeMaintenance>()), Times.Once);
        }

        [Fact]
        public async Task AddInvalidModel()
        {
            _mockChargeMaintenanceGateway.Setup(_ => _.AddAsync(It.IsAny<ChargeMaintenance>()))
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
                ex.Message.Should().Be("Value cannot be null. (Parameter 'chargeMaintenance')");
            }
        }
    }
}
