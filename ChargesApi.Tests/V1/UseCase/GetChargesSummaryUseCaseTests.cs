using ChargesApi.V1.Domain;
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
    public class GetChargesSummaryUseCaseTests
    {
        private readonly Mock<IChargesApiGateway> _mockChargesApiGateway;
        private readonly GetChargesSummaryUseCase _getChargesSummaryUseCase;

        public GetChargesSummaryUseCaseTests()
        {
            _mockChargesApiGateway = new Mock<IChargesApiGateway>();
            _getChargesSummaryUseCase = new GetChargesSummaryUseCase(_mockChargesApiGateway.Object);
        }
        [Fact]
        public async Task GetsAllChargesSummaryForBlock()
        {
            var entities = new List<Charge>()
                    {
                        new Charge
                        {
                            Id = new Guid("271b9a38-e78f-4a3f-81c0-4541bc5acc2c"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                            TargetType = TargetType.block,
                            ChargeGroup = ChargeGroup.tenants,
                            DetailedCharges = new List<DetailedCharges>()
                            {
                                new DetailedCharges
                                {
                                    Type = "service",
                                    SubType = "Block Cleaning",
                                    ChargeCode = "DCB",
                                    ChargeType = ChargeType.estate,
                                    StartDate = new DateTime(2021, 7, 2),
                                    EndDate = new DateTime(2021, 7, 4),
                                    Amount = 150,
                                    Frequency = "weekly"
                                },
                                new DetailedCharges
                                {
                                    Type = "service",
                                    SubType = "Heating",
                                    ChargeCode = "DCT",
                                    ChargeType = ChargeType.block,
                                    StartDate = new DateTime(2021, 7, 2),
                                    EndDate = new DateTime(2021, 7, 4),
                                    Amount = 120,
                                    Frequency = "weekly"
                                }
                            }
                        },
                        new Charge
                        {
                            Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                            TargetType = TargetType.block,
                            ChargeGroup = ChargeGroup.leaseholders,
                            DetailedCharges = new List<DetailedCharges>()
                            {
                                 new DetailedCharges
                                {
                                    Type = "service",
                                    SubType = "Block Cleaning",
                                    ChargeCode = "DCB",
                                    ChargeType = ChargeType.estate,
                                    StartDate = new DateTime(2021, 7, 2),
                                    EndDate = new DateTime(2021, 7, 4),
                                    Amount = 150,
                                    Frequency = "weekly"
                                },
                                new DetailedCharges
                                {
                                    Type = "service",
                                    SubType = "Heating",
                                    ChargeCode = "DCT",
                                    ChargeType = ChargeType.block,
                                    StartDate = new DateTime(2021, 7, 2),
                                    EndDate = new DateTime(2021, 7, 4),
                                    Amount = 120,
                                    Frequency = "weekly"
                                }
                            }
                        }
                    };

            _mockChargesApiGateway.Setup(x => x.GetAllChargesAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(entities);

            var result = await _getChargesSummaryUseCase.ExecuteAsync(new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"), "block")
                .ConfigureAwait(false);

            result.TargetId.Should().Be(new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"));
            result.TargetType.Should().Be(TargetType.block);
            result.TenantsCharges.Should().HaveCount(2);
            result.LeaseholdersCharges.Should().HaveCount(2);
        }
    }
}
