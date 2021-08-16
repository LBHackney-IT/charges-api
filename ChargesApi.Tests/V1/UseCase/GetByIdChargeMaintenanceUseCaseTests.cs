using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.UseCase
{
    public class GetByIdChargeMaintenanceUseCaseTests
    {
        private readonly Mock<IChargeMaintenanceApiGateway> _mockChargeMaintenanceGateway;
        private readonly GetByIdChargeMaintenanceUseCase _getByIdUseCase;

        public GetByIdChargeMaintenanceUseCaseTests()
        {
            _mockChargeMaintenanceGateway = new Mock<IChargeMaintenanceApiGateway>();
            _getByIdUseCase = new GetByIdChargeMaintenanceUseCase(_mockChargeMaintenanceGateway.Object);
        }
        [Fact]
        public async Task GetByIdValidIdReturnsCharge()
        {
            var domain = new ChargeMaintenance
            {
                Id = new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"),
                ChargesId = new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"),
                Reason = "Uplift",
                NewValue = new List<DetailedCharges>()
                    {
                        new DetailedCharges
                        {
                            Type = "service",
                            SubType = "water",
                            StartDate = new DateTime(2021, 7, 2),
                            EndDate = new DateTime(2021, 7, 4),
                            Amount = 150,
                            Frequency = "weekly"
                        }
                    },
                ExistingValue = new List<DetailedCharges>()
                    {
                        new DetailedCharges
                        {
                            Type = "service",
                            SubType = "water",
                            StartDate = new DateTime(2021, 7, 2),
                            EndDate = new DateTime(2021, 7, 4),
                            Amount = 120,
                            Frequency = "weekly"
                        }
                    },
                StartDate = new DateTime(2021, 7, 2),
                Status = ChargeMaintenanceStatus.Pending
            };

            var expectedResult = domain.ToResponse();

            _mockChargeMaintenanceGateway.Setup(_ => _.GetChargeMaintenanceByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(domain);

            var result = await _getByIdUseCase.ExecuteAsync(Guid.NewGuid())
                .ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetByIdInvalidIdReturnsCharge()
        {
            _mockChargeMaintenanceGateway.Setup(_ => _.GetChargeMaintenanceByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ChargeMaintenance) null);

            var result = await _getByIdUseCase.ExecuteAsync(Guid.NewGuid())
                .ConfigureAwait(false);

            result.Should().BeNull();
        }
    }
}
