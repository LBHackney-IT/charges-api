using AutoFixture;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Infrastructure.Entities;
using FluentAssertions;
using Xunit;

namespace ChargesApi.Tests.V1.Factories
{
    public class ChargesListFactoryTests
    {
        private readonly Fixture _fixture = new Fixture();
        [Fact]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var databaseEntity = _fixture.Create<ChargesListDbEntity>();

            var domain = databaseEntity.ToDomain();

            databaseEntity.Id.Should().Be(domain.Id);
            databaseEntity.ChargeCode.Should().Be(domain.ChargeCode);
            databaseEntity.ChargeName.Should().Be(domain.ChargeName);
            databaseEntity.ChargeGroup.Should().Be(domain.ChargeGroup);
            databaseEntity.ChargeType.Should().Be(domain.ChargeType);
        }

        [Fact]
        public void CanMapADomainEntityToADatabaseObject()
        {
            var domain = _fixture.Create<ChargesList>();

            var databaseEntity = domain.ToDatabase();

            databaseEntity.Id.Should().Be(domain.Id);
            databaseEntity.ChargeCode.Should().Be(domain.ChargeCode);
            databaseEntity.ChargeName.Should().Be(domain.ChargeName);
            databaseEntity.ChargeGroup.Should().Be(domain.ChargeGroup);
            databaseEntity.ChargeType.Should().Be(domain.ChargeType);
        }
        [Fact]
        public void CanMapARequestEntityToADomainObject()
        {
            var request = _fixture.Create<AddChargesListRequest>();

            var domainEntity = request.ToDomain();

            domainEntity.ChargeCode.Should().Be(request.ChargeCode);
            domainEntity.ChargeName.Should().Be(request.ChargeName);
            domainEntity.ChargeGroup.Should().Be(request.ChargeGroup);
            domainEntity.ChargeType.Should().Be(request.ChargeType);
        }
    }
}
