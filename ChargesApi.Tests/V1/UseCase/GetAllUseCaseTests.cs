using AutoFixture;
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
    public class GetAllUseCaseTests
    {
        private readonly Mock<IChargesApiGateway> _mockChargeGateway;
        private readonly GetAllUseCase _getAllUseCase;

        private readonly Fixture _fixture;

        public GetAllUseCaseTests()
        {
            _fixture = new Fixture();
            _mockChargeGateway = new Mock<IChargesApiGateway>();
            _getAllUseCase = new GetAllUseCase(_mockChargeGateway.Object);
        }

        [Fact]
        public async Task ShouldReturnAllValuesFromGateway()
        {
            var entities = new List<Charge>()
                    {
                        new Charge
                        {
                            Id = new Guid("271b9a38-e78f-4a3f-81c0-4541bc5acc2c"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                            TargetType = TargetType.Dwelling,
                            DetailedCharges = new List<DetailedCharges>()
                        },
                        new Charge
                        {
                            Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                            TargetType = TargetType.Dwelling,
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
                        }
                    };

            _mockChargeGateway.Setup(x => x.GetAllChargesAsync(It.IsAny<Guid>()))
                .ReturnsAsync(entities);

            var expectedResult = entities.ToResponse();

            var result = await _getAllUseCase.ExecuteAsync(new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"))
                .ConfigureAwait(false);

            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldReturnLatestVersion()
        {
            var targetId = Guid.NewGuid();
            var chargeList = _fixture.Build<Charge>()
                .OmitAutoProperties()
                .With(c => c.TargetId, targetId)
                .With(c => c.VersionId, 0)
                .CreateMany(20).ToList();

            foreach (var charge in chargeList.Take(10).ToList())
            {
                charge.VersionId = 1;
            }

            _mockChargeGateway.Setup(g => g.GetAllChargesAsync(It.IsAny<Guid>())).ReturnsAsync(chargeList);

            var result = await _getAllUseCase.ExecuteAsync(targetId).ConfigureAwait(false);

            result.Should().HaveCount(10);
        }
    }
}
