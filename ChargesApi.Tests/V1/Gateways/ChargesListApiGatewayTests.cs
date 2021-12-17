using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using AutoFixture;
using ChargesApi.Tests.V1.Helper;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.Infrastructure;
using ChargesApi.V1.Infrastructure.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.Gateways
{
    public class ChargesListApiGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IDynamoDBContext> _dynamoDbContext;
        private readonly Mock<IAmazonDynamoDB> _dynamoDb;
        private readonly ChargesListApiGateway _gateway;
        public ChargesListApiGatewayTests()
        {
            _dynamoDbContext = new Mock<IDynamoDBContext>();
            _dynamoDb = new Mock<IAmazonDynamoDB>();
            _gateway = new ChargesListApiGateway(_dynamoDbContext.Object, _dynamoDb.Object);
        }
        [Fact]
        public async Task GetByIdDbReturnsEntityReturnsTheEntity()
        {
            var id = Guid.NewGuid();
            var dbEntity = _fixture.Create<ChargesListDbEntity>();

            dbEntity.Id = id;

            _dynamoDbContext.Setup(x => x.LoadAsync<ChargesListDbEntity>(It.IsAny<Guid>(), default))
                     .ReturnsAsync(dbEntity);

            var response = await _gateway.GetChargesListByIdAsync(id).ConfigureAwait(false);

            _dynamoDbContext.Verify(x => x.LoadAsync<ChargesListDbEntity>(It.IsAny<Guid>(), default), Times.Once);

            dbEntity.Should().NotBeNull();
            dbEntity.Id.Should().Be(id);
        }
        [Fact]
        public async Task GetByIdDbReturnsNullReturnsNull()
        {
            _dynamoDbContext.Setup(_ => _.LoadAsync<ChargesListDbEntity>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ChargesListDbEntity) null);

            var response = await _gateway.GetChargesListByIdAsync(Guid.NewGuid()).ConfigureAwait(false);

            response.Should().BeNull();
        }
        [Fact]
        public async Task AddWithValidModelWorksOnce()
        {
            _dynamoDbContext.Setup(_ => _.SaveAsync(It.IsAny<ChargesListDbEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var domain = _fixture.Create<ChargesList>();

            await _gateway.AddAsync(domain).ConfigureAwait(false);

            _dynamoDbContext.Verify(_ => _.SaveAsync(It.IsAny<ChargesListDbEntity>(), default), Times.Once);
        }
        [Fact]
        public async Task GetAllDbReturnsZeroItemsReturnsEmptyList()
        {
            QueryResponse response = FakeDataHelper.MockQueryResponse<ChargesListResponse>();

            _dynamoDb.Setup(p => p.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueryResponse());

            var result = await _gateway.GetAllChargesListAsync(ChargeGroup.Tenants.ToString(), ChargeType.Estate.ToString()).ConfigureAwait(false);
            result.Should().NotBeNull();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDbReturnsItemsReturnsList()
        {
            QueryResponse response = FakeDataHelper.MockQueryResponse<ChargesListResponse>();

            _dynamoDb.Setup(p => p.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _gateway.GetAllChargesListAsync(ChargeGroup.Tenants.ToString(), ChargeType.Estate.ToString()).ConfigureAwait(false);
            _dynamoDb.Verify(x => x.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(response.ToChargesListDomain());
        }


    }
}
