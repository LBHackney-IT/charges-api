using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Infrastructure.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ChargesApi.Tests.V1.Factories
{

    public class ChargeFactoryTests
    {
        [Fact]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var databaseEntity = new ChargeDbEntity()
            {
                Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
                TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
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

            var domain = databaseEntity.ToDomain();

            databaseEntity.Id.Should().Be(domain.Id);
            databaseEntity.TargetId.Should().Be(domain.TargetId);
            databaseEntity.TargetType.Should().Be(domain.TargetType);

            var entityDetailedCharges = databaseEntity.DetailedCharges.ToList();
            var domainDetailedCharges = domain.DetailedCharges.ToList();

            domainDetailedCharges.Should().NotBeNullOrEmpty();
            domainDetailedCharges.Should().HaveCount(1);

            entityDetailedCharges[0].Type.Should().BeEquivalentTo(domainDetailedCharges[0].Type);
            entityDetailedCharges[0].SubType.Should().BeEquivalentTo(domainDetailedCharges[0].SubType);
            entityDetailedCharges[0].Frequency.Should().BeEquivalentTo(domainDetailedCharges[0].Frequency);
            entityDetailedCharges[0].Amount.Should().Be(domainDetailedCharges[0].Amount);
            entityDetailedCharges[0].StartDate.Should().Be(domainDetailedCharges[0].StartDate);
            entityDetailedCharges[0].EndDate.Should().Be(domainDetailedCharges[0].EndDate);
        }

        [Fact]
        public void CanMapADomainEntityToADatabaseObject()
        {
            var domain = new Charge()
            {
                Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
                TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
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

            var databaseEntity = domain.ToDatabase();

            domain.Id.Should().Be(databaseEntity.Id);
            domain.TargetId.Should().Be(databaseEntity.TargetId);
            domain.TargetType.Should().Be(databaseEntity.TargetType);

            var domainDetailedCharges = domain.DetailedCharges.ToList();
            var entityDetailedCharges = databaseEntity.DetailedCharges.ToList();

            entityDetailedCharges.Should().NotBeNullOrEmpty();
            entityDetailedCharges.Should().HaveCount(1);

            domainDetailedCharges[0].Type.Should().BeEquivalentTo(entityDetailedCharges[0].Type);
            domainDetailedCharges[0].SubType.Should().BeEquivalentTo(entityDetailedCharges[0].SubType);
            domainDetailedCharges[0].Frequency.Should().BeEquivalentTo(entityDetailedCharges[0].Frequency);
            domainDetailedCharges[0].Amount.Should().Be(entityDetailedCharges[0].Amount);
            domainDetailedCharges[0].StartDate.Should().Be(entityDetailedCharges[0].StartDate);
            domainDetailedCharges[0].EndDate.Should().Be(entityDetailedCharges[0].EndDate);
        }
    }
}
