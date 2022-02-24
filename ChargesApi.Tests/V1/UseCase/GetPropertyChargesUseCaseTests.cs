using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChargesApi.Tests.V1.UseCase
{
    public class GetPropertyChargesUseCaseTests
    {
        private readonly Mock<IChargesApiGateway> _mockChargesApiGateway;
        private readonly GetPropertyChargesUseCase _getPropertyChargesUseCase;

        public GetPropertyChargesUseCaseTests()
        {
            _mockChargesApiGateway = new Mock<IChargesApiGateway>();
            _getPropertyChargesUseCase = new GetPropertyChargesUseCase(_mockChargesApiGateway.Object);
        }

        [Fact]
        public async Task GetsPropertyCharges()
        {
            var entities = new List<Charge>()
                    {
                        new Charge
                        {
                            Id = Guid.NewGuid(),
                            TargetId = Guid.NewGuid(),
                            TargetType = TargetType.Block,
                            ChargeGroup = ChargeGroup.Tenants,
                            ChargeYear = 2022,
                            DetailedCharges = new List<DetailedCharges>()
                            {
                                new DetailedCharges
                                {
                                    Type = "Service",
                                    SubType = "Block Cleaning",
                                    ChargeCode = "DCB",
                                    ChargeType = ChargeType.Estate,
                                    StartDate = new DateTime(2021, 7, 2),
                                    EndDate = new DateTime(2021, 7, 4),
                                    Amount = 150,
                                    Frequency = "Weekly"
                                },
                                new DetailedCharges
                                {
                                    Type = "Service",
                                    SubType = "Heating",
                                    ChargeCode = "DCT",
                                    ChargeType = ChargeType.Block,
                                    StartDate = new DateTime(2021, 7, 2),
                                    EndDate = new DateTime(2021, 7, 4),
                                    Amount = 120,
                                    Frequency = "Weekly"
                                }
                            }
                        },
                        new Charge
                        {
                            Id = Guid.NewGuid(),
                            TargetId = Guid.NewGuid(),
                            TargetType = TargetType.Block,
                            ChargeGroup = ChargeGroup.Tenants,
                            DetailedCharges = new List<DetailedCharges>()
                            {
                                 new DetailedCharges
                                {
                                    Type = "Service",
                                    SubType = "Block Cleaning",
                                    ChargeCode = "DCB",
                                    ChargeType = ChargeType.Estate,
                                    StartDate = new DateTime(2021, 7, 2),
                                    EndDate = new DateTime(2021, 7, 4),
                                    Amount = 150,
                                    Frequency = "Weekly"
                                },
                                new DetailedCharges
                                {
                                    Type = "Service",
                                    SubType = "Heating",
                                    ChargeCode = "DCT",
                                    ChargeType = ChargeType.Block,
                                    StartDate = new DateTime(2021, 7, 2),
                                    EndDate = new DateTime(2021, 7, 4),
                                    Amount = 120,
                                    Frequency = "Weekly"
                                }
                            }
                        }
                    };

            var queryParams = new PropertyChargesQueryParameters()
            {
                ChargeYear = 2022,
                ChargeSubGroup = ChargeSubGroup.Actual,
                ChargeGroup = ChargeGroup.Tenants
            };

            _mockChargesApiGateway.Setup(x => x.GetChargesAsync(queryParams))
                .ReturnsAsync(entities);

            var result = await _getPropertyChargesUseCase.ExecuteAsync(queryParams)
                .ConfigureAwait(false);

            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);
        }
    }
}
