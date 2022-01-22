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
        private const string Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ0ZXN0IiwiaWF0IjoxNjM5NDIyNzE4LCJleHAiOjE5ODY1Nzc5MTgsImF1ZCI6InRlc3QiLCJzdWIiOiJ0ZXN0IiwiZ3JvdXBzIjpbInNvbWUtdmFsaWQtZ29vZ2xlLWdyb3VwIiwic29tZS1vdGhlci12YWxpZC1nb29nbGUtZ3JvdXAiXSwibmFtZSI6InRlc3RpbmcifQ.IcpQ00PGVgksXkR_HFqWOakgbQ_PwW9dTVQu4w77tmU";

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

            _dynamoDbContext.Setup(x => x.LoadAsync<ChargesListDbEntity>(It.IsAny<string>(), It.IsAny<Guid>(), default))
                     .ReturnsAsync(dbEntity);

            var response = await _gateway.GetChargesListByIdAsync(id, dbEntity.ChargeCode).ConfigureAwait(false);

            _dynamoDbContext.Verify(x => x.LoadAsync<ChargesListDbEntity>(It.IsAny<string>(), It.IsAny<Guid>(), default), Times.Once);

            dbEntity.Should().NotBeNull();
            dbEntity.Id.Should().Be(id);
        }
        [Fact]
        public async Task GetByIdDbReturnsNullReturnsNull()
        {
            _dynamoDbContext.Setup(_ => _.LoadAsync<ChargesListDbEntity>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ChargesListDbEntity) null);

            var response = await _gateway.GetChargesListByIdAsync(Guid.NewGuid(), "DCB").ConfigureAwait(false);

            response.Should().BeNull();
        }
        [Fact]
        public async Task AddWithValidModelWorksOnce()
        {
            _dynamoDbContext.Setup(_ => _.SaveAsync(It.IsAny<ChargesListDbEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var domain = _fixture.Create<ChargesList>();

            await _gateway.AddAsync(domain, Token).ConfigureAwait(false);

            _dynamoDbContext.Verify(_ => _.SaveAsync(It.IsAny<ChargesListDbEntity>(), default), Times.Once);
        }
        [Fact]
        public async Task GetAllDbReturnsZeroItemsReturnsEmptyList()
        {
            QueryResponse response = FakeDataHelper.MockQueryResponse<ChargesListResponse>();

            _dynamoDb.Setup(p => p.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueryResponse());

            var result = await _gateway.GetAllChargesListAsync("DCB").ConfigureAwait(false);
            result.Should().NotBeNull();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDbReturnsItemsReturnsList()
        {
            QueryResponse response = FakeDataHelper.MockQueryResponse<ChargesListResponse>();

            _dynamoDb.Setup(p => p.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _gateway.GetAllChargesListAsync("DCB").ConfigureAwait(false);
            _dynamoDb.Verify(x => x.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(response.ToChargesListDomain());
        }


    }
}
