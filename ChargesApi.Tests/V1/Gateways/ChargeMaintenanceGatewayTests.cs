using Amazon.DynamoDBv2.DataModel;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.Infrastructure.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.Gateways
{
    public class ChargeMaintenanceGatewayTests
    {
        private readonly Mock<IDynamoDBContext> _dynamoDb;
        private readonly ChargeMaintenanceGateway _gateway;
        public ChargeMaintenanceGatewayTests()
        {
            _dynamoDb = new Mock<IDynamoDBContext>();
            _gateway = new ChargeMaintenanceGateway(_dynamoDb.Object);
        }
        [Fact]
        public async Task AddChargeMaintenanceWithValidModel()
        {
            _dynamoDb.Setup(_ => _.SaveAsync(It.IsAny<ChargesMaintenanceDbEntity>(), It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

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

            await _gateway.AddAsync(domain).ConfigureAwait(false);

            _dynamoDb.Verify(_ => _.SaveAsync(It.IsAny<ChargesMaintenanceDbEntity>(), default), Times.Once);
        }

        [Fact]
        public async Task AddChargeWithInvalidModel()
        {
            _dynamoDb.Setup(_ => _.SaveAsync(It.IsAny<ChargesMaintenanceDbEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _gateway.AddAsync(null).ConfigureAwait(false);

            _dynamoDb.Verify(_ => _.SaveAsync(It.IsAny<ChargesMaintenanceDbEntity>(), default), Times.Once);
        }
        [Fact]
        public async Task GetChargeMaintenanceByIdReturnsNullIfEntityDoesntExist()
        {
            _dynamoDb.Setup(_ => _.LoadAsync<ChargesMaintenanceDbEntity>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ChargesMaintenanceDbEntity());

            var charge = await _gateway.GetChargeMaintenanceByIdAsync(new Guid("40e69b91-9f2a-4d4c-b0f8-c61250d88c89"))
                .ConfigureAwait(false);

            charge.NewValue.Should().BeNull();
            charge.ExistingValue.Should().BeNull();
        }

        [Fact]
        public async Task GetChargeByIdReturnsAssetSummaryIfItExists()
        {
            var entiy = new ChargesMaintenanceDbEntity
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

            _dynamoDb.Setup(_ => _.LoadAsync<ChargesMaintenanceDbEntity>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult(entiy));
            var chargeMaintenance = await _gateway.GetChargeMaintenanceByIdAsync(new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"))
                .ConfigureAwait(false);

            chargeMaintenance.Should().NotBeNull();

            chargeMaintenance.Id.Should().Be(new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"));
            chargeMaintenance.ChargesId.Should().Be(new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"));
            chargeMaintenance.Status.Should().Be(ChargeMaintenanceStatus.Pending);
            chargeMaintenance.StartDate.Should().Be(new DateTime(2021, 7, 2));
            chargeMaintenance.Reason.Should().Be("Uplift");

            chargeMaintenance.ExistingValue.Should().HaveCount(1);
            chargeMaintenance.NewValue.Should().HaveCount(1);


            var detailedChargesExisting = chargeMaintenance.ExistingValue.ToList();
            detailedChargesExisting[0].Type.Should().BeEquivalentTo("service");
            detailedChargesExisting[0].SubType.Should().BeEquivalentTo("water");
            detailedChargesExisting[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            detailedChargesExisting[0].EndDate.Should().Be(new DateTime(2021, 7, 4));
            detailedChargesExisting[0].Amount.Should().Be(120);
            detailedChargesExisting[0].Frequency.Should().BeEquivalentTo("weekly");

            var detailedChargesNew = chargeMaintenance.NewValue.ToList();
            detailedChargesNew[0].Type.Should().BeEquivalentTo("service");
            detailedChargesNew[0].SubType.Should().BeEquivalentTo("water");
            detailedChargesNew[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            detailedChargesNew[0].EndDate.Should().Be(new DateTime(2021, 7, 4));
            detailedChargesNew[0].Amount.Should().Be(150);
            detailedChargesNew[0].Frequency.Should().BeEquivalentTo("weekly");
        }
    }
}
