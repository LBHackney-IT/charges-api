using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using ChargesApi.Tests.V1.Helper;
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
using Microsoft.Extensions.Configuration;
using Xunit;
using Microsoft.Extensions.Logging;
using AutoFixture;

namespace ChargesApi.Tests.V1.Gateways
{
    [Collection("LogCall collection")]
    public class DynamoDbGatewayTests
    {
        private readonly Mock<IDynamoDBContext> _dynamoDb;
        private readonly Mock<IAmazonDynamoDB> _amazonDynamoDb;
        private readonly ChargesApiGateway _gateway;
        private readonly Mock<ILogger<ChargesApiGateway>> _logger;
        private readonly Fixture _fixture;

        public DynamoDbGatewayTests()
        {
            _fixture = new Fixture();
            _dynamoDb = new Mock<IDynamoDBContext>();
            _amazonDynamoDb = new Mock<IAmazonDynamoDB>();
            _logger = new Mock<ILogger<ChargesApiGateway>>();
            _gateway = new ChargesApiGateway(_dynamoDb.Object, _amazonDynamoDb.Object);
        }
        [Fact]
        public async Task GetChargeByIdReturnsNullIfEntityDoesntExist()
        {
            _dynamoDb.Setup(p => p.LoadAsync<ChargeDbEntity>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ChargeDbEntity) null);

            var charge = await _gateway.GetChargeByIdAsync(new Guid("40e69b91-9f2a-4d4c-b0f8-c61250d88c89"), Guid.NewGuid())
                .ConfigureAwait(false);

            charge.Should().BeNull();
        }

        [Fact]
        public async Task GetChargeByIdReturnsChargesIfItExists()
        {
            var chargeObj = new ChargeDbEntity
            {
                Id = new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"),
                TargetId = new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"),
                TargetType = TargetType.Dwelling,
                DetailedCharges = new List<DetailedCharges>
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

            _dynamoDb.Setup(p => p.LoadAsync<ChargeDbEntity>(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(chargeObj);

            var charge = await _gateway.GetChargeByIdAsync(new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"), new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"))
                .ConfigureAwait(false);

            charge.Should().NotBeNull();

            charge.Id.Should().Be(new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"));
            charge.TargetId.Should().Be(new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"));
            charge.TargetType.Should().Be(TargetType.Dwelling);
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
            QueryResponse response = FakeDataHelper.MockQueryResponse<Charge>();
            _amazonDynamoDb.Setup(p => p.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(response);

            var charges = await _gateway.GetAllChargesAsync(new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"))
                .ConfigureAwait(false);

            charges.Should().NotBeNull();
            charges.Should().HaveCount(1);

            charges[0].TargetType.Should().Be(TargetType.Dwelling);
            charges[0].DetailedCharges.Should().NotBeNull();
            charges[0].DetailedCharges.Should().HaveCount(9);

            var detailedCharges = charges[0].DetailedCharges.ToList();
            detailedCharges[0].Type.Should().NotBeNull();
            detailedCharges[0].SubType.Should().NotBeNull();
            detailedCharges[0].Amount.Should().BeGreaterThan(0);
            detailedCharges[0].Frequency.Should().NotBeNull();
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
                TargetType = TargetType.Dwelling,
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
        public async Task UpdateChargeWithValidModel()
        {
            _dynamoDb.Setup(_ => _.SaveAsync(It.IsAny<ChargeDbEntity>(), It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);
            var domain = new Charge
            {
                Id = new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"),
                TargetId = new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"),
                TargetType = TargetType.Dwelling,
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
        public async Task RemoveChargeIfItExist()
        {
            _dynamoDb.Setup(_ => _.DeleteAsync(It.IsAny<ChargeDbEntity>(), default))
                .Returns(Task.CompletedTask);
            var domain = new Charge
            {
                Id = new Guid("4976341d-f5fe-40a5-a9a0-6aa88a3692d2"),
                TargetId = new Guid("a361a7f2-fa89-4131-a66e-9434e8425a7c"),
                TargetType = TargetType.Dwelling,
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

        [Fact]
        public async Task AddTransactionBatchAsync()
        {
            var entities = new List<Charge>()
            {
                        new Charge
                        {
                            Id = new Guid("271b9a38-e78f-4a3f-81c0-4541bc5acc2c"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                            TargetType = TargetType.Dwelling,
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
                        },
                        new Charge
                        {
                            Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                            TargetType = TargetType.Dwelling,
                            ChargeGroup = ChargeGroup.Leaseholders,
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

            _amazonDynamoDb.Setup(p => p.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new TransactWriteItemsResponse());

            var result = await _gateway.AddTransactionBatchAsync(entities)
                .ConfigureAwait(false);
            result.Should().BeTrue();

        }
        private static List<Charge> GetCharges()
        {
            return new List<Charge>()
            {
                        new Charge
                        {
                            Id = new Guid("271b9a38-e78f-4a3f-81c0-4541bc5acc2c"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                            TargetType = TargetType.Dwelling,
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
                        },
                        new Charge
                        {
                            Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                            TargetType = TargetType.Dwelling,
                            ChargeGroup = ChargeGroup.Leaseholders,
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

        }
        [Fact]
        public void AddTransactionBatchAsyncWithException()
        {
            var entities = GetCharges();
            var ex = new TransactionCanceledException("test");
            _amazonDynamoDb.Setup(p => p.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new TransactWriteItemsResponse());

            var result = _gateway.AddTransactionBatchAsync(null)
                .ConfigureAwait(false);

            Func<Task<bool>> func =
                async () => await _gateway.AddTransactionBatchAsync(null)
                .ConfigureAwait(false);

            func.Should().ThrowAsync<Exception>();


        }
    }
}
