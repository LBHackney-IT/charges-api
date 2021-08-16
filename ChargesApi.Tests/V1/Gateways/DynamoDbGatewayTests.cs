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
    public class DynamoDbGatewayTests
    {
        private readonly Mock<IDynamoDBContext> _dynamoDb;
        private readonly Mock<DynamoDbContextWrapper> _wrapper;
        private readonly DynamoDbGateway _gateway;

        public DynamoDbGatewayTests()
        {
            _dynamoDb = new Mock<IDynamoDBContext>();
            _wrapper = new Mock<DynamoDbContextWrapper>();
            _gateway = new DynamoDbGateway(_dynamoDb.Object, _wrapper.Object);
        }

        [Fact]
        public async Task GetChargeByIdReturnsNullIfEntityDoesntExist()
        {
            _wrapper.Setup(_ => _.ScanAsync(
                It.IsAny<IDynamoDBContext>(),
                It.IsAny<IEnumerable<ScanCondition>>(),
                It.IsAny<DynamoDBOperationConfig>()))
                .ReturnsAsync(new List<ChargeDbEntity>());

            var charge = await _gateway.GetChargeByIdAsync(new Guid("40e69b91-9f2a-4d4c-b0f8-c61250d88c89"))
                .ConfigureAwait(false);

            charge.Should().BeNull();
        }

        [Fact]
        public async Task GetChargeByIdReturnsAssetSummaryIfItExists()
        {
            _wrapper.Setup(_ => _.ScanAsync(
                It.IsAny<IDynamoDBContext>(),
                It.IsAny<IEnumerable<ScanCondition>>(),
                It.IsAny<DynamoDBOperationConfig>()))
                .ReturnsAsync(new List<ChargeDbEntity>()
                {
                    new ChargeDbEntity
                    {
                        Id = new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"),
                        TargetId = new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"),
                        TargetType = TargetType.Asset,
                        DetailedCharges = new List<DetailedCharges>()
                        {
                            new DetailedCharges
                            {
                                Type = "Type",
                                SubType = "SubType",
                                StartDate = new DateTime(2021, 7, 2),
                                EndDate = new DateTime(2021, 7, 3),
                                Amount = 150,
                                Frequency = "Frequency"
                            }
                        }
                    }
                });

            var charge = await _gateway.GetChargeByIdAsync(new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"))
                .ConfigureAwait(false);

            charge.Should().NotBeNull();

            charge.Id.Should().Be(new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"));
            charge.TargetId.Should().Be(new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"));
            charge.TargetType.Should().Be(TargetType.Asset);
            charge.DetailedCharges.Should().NotBeNull();
            charge.DetailedCharges.Should().HaveCount(1);

            var detailedCharges = charge.DetailedCharges.ToList();
            detailedCharges[0].Type.Should().BeEquivalentTo("Type");
            detailedCharges[0].SubType.Should().BeEquivalentTo("SubType");
            detailedCharges[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            detailedCharges[0].EndDate.Should().Be(new DateTime(2021, 7, 3));
            detailedCharges[0].Amount.Should().Be(150);
            detailedCharges[0].Frequency.Should().BeEquivalentTo("Frequency");
        }

        [Fact]
        public async Task GetAllChargesByTypeAndTargetIdReturnsList()
        {
            _wrapper.Setup(_ => _.ScanAsync(
                It.IsAny<IDynamoDBContext>(),
                It.IsAny<IEnumerable<ScanCondition>>(),
                It.IsAny<DynamoDBOperationConfig>()))
                .ReturnsAsync(new List<ChargeDbEntity>()
                {
                    new ChargeDbEntity
                    {
                        Id = new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"),
                        TargetId = new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"),
                        TargetType = TargetType.Asset,
                        DetailedCharges = new List<DetailedCharges>()
                        {
                            new DetailedCharges
                            {
                                Type = "Type",
                                SubType = "SubType",
                                StartDate = new DateTime(2021, 7, 2),
                                EndDate = new DateTime(2021, 7, 3),
                                Amount = 150,
                                Frequency = "Frequency"
                            }
                        }
                    },
                    new ChargeDbEntity
                    {
                        Id = new Guid("dde1c761-800d-4ba8-815e-a6537f28f65a"),
                        TargetId = new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"),
                        TargetType = TargetType.Asset,
                        DetailedCharges = new List<DetailedCharges>()
                    }
                });

            var charges = await _gateway.GetAllChargesAsync("Asset", new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"))
                .ConfigureAwait(false);

            charges.Should().NotBeNull();
            charges.Should().HaveCount(2);

            charges[0].Id.Should().Be(new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"));
            charges[0].TargetId.Should().Be(new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"));
            charges[0].TargetType.Should().Be(TargetType.Asset);
            charges[0].DetailedCharges.Should().NotBeNull();
            charges[0].DetailedCharges.Should().HaveCount(1);

            var detailedCharges = charges[0].DetailedCharges.ToList();
            detailedCharges[0].Type.Should().BeEquivalentTo("Type");
            detailedCharges[0].SubType.Should().BeEquivalentTo("SubType");
            detailedCharges[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            detailedCharges[0].EndDate.Should().Be(new DateTime(2021, 7, 3));
            detailedCharges[0].Amount.Should().Be(150);
            detailedCharges[0].Frequency.Should().BeEquivalentTo("Frequency");

            charges[1].Id.Should().Be(new Guid("dde1c761-800d-4ba8-815e-a6537f28f65a"));
            charges[1].TargetId.Should().Be(new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"));
            charges[1].TargetType.Should().Be(TargetType.Asset);
            charges[1].DetailedCharges.Should().NotBeNull();
            charges[1].DetailedCharges.Should().HaveCount(0);
        }

        [Fact]
        public async Task AddChargeWithValidModel()
        {
            _dynamoDb.Setup(_ => _.SaveAsync(It.IsAny<ChargeDbEntity>(), It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            var domain = new Charge
            {
                Id = new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"),
                TargetId = new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"),
                TargetType = TargetType.Asset,
                DetailedCharges = new List<DetailedCharges>()
                {
                    new DetailedCharges
                    {
                        Type = "Type",
                        SubType = "SubType",
                        StartDate = new DateTime(2021, 7, 2),
                        EndDate = new DateTime(2021, 7, 3),
                        Amount = 150,
                        Frequency = "Frequency"
                    }
                }
            };

            await _gateway.AddAsync(domain).ConfigureAwait(false);

            _dynamoDb.Verify(_ => _.SaveAsync(It.IsAny<ChargeDbEntity>(), default), Times.Once);
        }

        [Fact]
        public async Task AddChargeWithInvalidModel()
        {
            _dynamoDb.Setup(_ => _.SaveAsync(It.IsAny<ChargeDbEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _gateway.AddAsync(null).ConfigureAwait(false);

            _dynamoDb.Verify(_ => _.SaveAsync(It.IsAny<ChargeDbEntity>(), default), Times.Once);
        }

        [Fact]
        public async Task UpdateChargeWithValidModel()
        {
            _dynamoDb.Setup(_ => _.SaveAsync(It.IsAny<ChargeDbEntity>(), It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            var domain = new Charge
            {
                Id = new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"),
                TargetId = new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"),
                TargetType = TargetType.Asset,
                DetailedCharges = new List<DetailedCharges>()
                {
                    new DetailedCharges
                    {
                        Type = "Type",
                        SubType = "SubType",
                        StartDate = new DateTime(2021, 7, 2),
                        EndDate = new DateTime(2021, 7, 3),
                        Amount = 150,
                        Frequency = "Frequency"
                    }
                }
            };

            await _gateway.AddAsync(domain).ConfigureAwait(false);

            _dynamoDb.Verify(_ => _.SaveAsync(It.IsAny<ChargeDbEntity>(), default), Times.Once);
        }

        [Fact]
        public async Task UpdateChargeWithInvalidModel()
        {
            _dynamoDb.Setup(_ => _.SaveAsync(It.IsAny<ChargeDbEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _gateway.AddAsync(null).ConfigureAwait(false);

            _dynamoDb.Verify(_ => _.SaveAsync(It.IsAny<ChargeDbEntity>(), default), Times.Once);
        }

        [Fact]
        public async Task RemoveChargeIfItExist()
        {
            _dynamoDb.Setup(_ => _.DeleteAsync(It.IsAny<ChargeDbEntity>(), default))
                .Returns(Task.CompletedTask);

            var domain = new Charge
            {
                Id = new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"),
                TargetId = new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"),
                TargetType = TargetType.Asset,
                DetailedCharges = new List<DetailedCharges>()
                {
                    new DetailedCharges
                    {
                        Type = "Type",
                        SubType = "SubType",
                        StartDate = new DateTime(2021, 7, 2),
                        EndDate = new DateTime(2021, 7, 3),
                        Amount = 150,
                        Frequency = "Frequency"
                    }
                }
            };

            await _gateway.RemoveAsync(domain).ConfigureAwait(false);

            _dynamoDb.Verify(_ => _.DeleteAsync(It.IsAny<ChargeDbEntity>(), default), Times.Once);
        }

        [Fact]
        public async Task RemoveChargeIfEntityDoesntExist()
        {
            _dynamoDb.Setup(_ => _.DeleteAsync(It.IsAny<ChargeDbEntity>(), default))
                .Returns(Task.CompletedTask);

            await _gateway.RemoveAsync(null).ConfigureAwait(false);

            _dynamoDb.Verify(_ => _.DeleteAsync(It.IsAny<ChargeDbEntity>(), default), Times.Once);
        }
    }
}
