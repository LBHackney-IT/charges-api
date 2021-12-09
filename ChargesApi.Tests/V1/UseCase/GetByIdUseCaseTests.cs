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
    public class GetByIdUseCaseTests
    {
        private readonly Mock<IChargesApiGateway> _mockGateway;
        private readonly GetByIdUseCase _getByIdUseCase;

        public GetByIdUseCaseTests()
        {
            _mockGateway = new Mock<IChargesApiGateway>();
            _getByIdUseCase = new GetByIdUseCase(_mockGateway.Object);
        }

        [Fact]
        public async Task GetByIdValidIdReturnsCharge()
        {
            var domain = new Charge
            {
                Id = new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"),
                TargetId = new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"),
                TargetType = TargetType.Asset,
                DetailedCharges = new List<DetailedCharges>()
                {
                    new DetailedCharges
                    {
                        Type = "Type",
                        SubType = "SubType",
                        StartDate = new DateTime(2021, 7, 2),
                        EndDate = new DateTime(2021, 7, 4),
                        Amount = 150,
                        Frequency = "Frequency"
                    }
                }
            };

            var expectedResult = domain.ToResponse();

            _mockGateway.Setup(_ => _.GetChargeByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(domain);

            var result = await _getByIdUseCase.ExecuteAsync(Guid.NewGuid())
                .ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetByIdInvalidIdReturnsCharge()
        {
            _mockGateway.Setup(_ => _.GetChargeByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Charge) null);

            var result = await _getByIdUseCase.ExecuteAsync(Guid.NewGuid())
                .ConfigureAwait(false);

            result.Should().BeNull();
        }
    }
}
