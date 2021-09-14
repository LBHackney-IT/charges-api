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
    public class ChargeMaintenanceFactoryTests
    {
        [Fact]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var databaseEntity = new ChargesMaintenanceDbEntity()
            {
                Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
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
                Status = ChargeMaintenanceStatus.pending
            };

            var domain = databaseEntity.ToDomain();

            databaseEntity.Id.Should().Be(domain.Id);
            databaseEntity.ChargesId.Should().Be(domain.ChargesId);
            databaseEntity.Status.Should().Be(domain.Status);

            var existingCharges = databaseEntity.ExistingValue.ToList();
            var domainDetailedCharges = domain.ExistingValue.ToList();

            domainDetailedCharges.Should().NotBeNullOrEmpty();
            domainDetailedCharges.Should().HaveCount(1);

            existingCharges[0].Type.Should().BeEquivalentTo(domainDetailedCharges[0].Type);
            existingCharges[0].SubType.Should().BeEquivalentTo(domainDetailedCharges[0].SubType);
            existingCharges[0].Frequency.Should().BeEquivalentTo(domainDetailedCharges[0].Frequency);
            existingCharges[0].Amount.Should().Be(domainDetailedCharges[0].Amount);
            existingCharges[0].StartDate.Should().Be(domainDetailedCharges[0].StartDate);
            existingCharges[0].EndDate.Should().Be(domainDetailedCharges[0].EndDate);

            var newCharges = databaseEntity.NewValue.ToList();
            var domainDetailedChargesNew = domain.NewValue.ToList();

            domainDetailedChargesNew.Should().NotBeNullOrEmpty();
            domainDetailedChargesNew.Should().HaveCount(1);

            newCharges[0].Type.Should().BeEquivalentTo(domainDetailedChargesNew[0].Type);
            newCharges[0].SubType.Should().BeEquivalentTo(domainDetailedChargesNew[0].SubType);
            newCharges[0].Frequency.Should().BeEquivalentTo(domainDetailedChargesNew[0].Frequency);
            newCharges[0].Amount.Should().Be(domainDetailedChargesNew[0].Amount);
            newCharges[0].StartDate.Should().Be(domainDetailedChargesNew[0].StartDate);
            newCharges[0].EndDate.Should().Be(domainDetailedChargesNew[0].EndDate);
        }

        [Fact]
        public void CanMapADomainEntityToADatabaseObject()
        {
            var domain = new ChargeMaintenance()
            {
                Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
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
                Status = ChargeMaintenanceStatus.pending
            };

            var databaseEntity = domain.ToDatabase();

            domain.Id.Should().Be(databaseEntity.Id);
            domain.ChargesId.Should().Be(databaseEntity.ChargesId);
            domain.Status.Should().Be(databaseEntity.Status);
            domain.Reason.Should().Be(databaseEntity.Reason);
            domain.StartDate.Should().Be(databaseEntity.StartDate);

            var domainExistingCharges = domain.ExistingValue.ToList();
            var entityDetailedCharges = databaseEntity.ExistingValue.ToList();

            entityDetailedCharges.Should().NotBeNullOrEmpty();
            entityDetailedCharges.Should().HaveCount(1);

            domainExistingCharges[0].Type.Should().BeEquivalentTo(entityDetailedCharges[0].Type);
            domainExistingCharges[0].SubType.Should().BeEquivalentTo(entityDetailedCharges[0].SubType);
            domainExistingCharges[0].Frequency.Should().BeEquivalentTo(entityDetailedCharges[0].Frequency);
            domainExistingCharges[0].Amount.Should().Be(entityDetailedCharges[0].Amount);
            domainExistingCharges[0].StartDate.Should().Be(entityDetailedCharges[0].StartDate);
            domainExistingCharges[0].EndDate.Should().Be(entityDetailedCharges[0].EndDate);

            var domainNewCharges = domain.ExistingValue.ToList();
            var entityDetailedNewCharges = databaseEntity.ExistingValue.ToList();

            entityDetailedCharges.Should().NotBeNullOrEmpty();
            entityDetailedCharges.Should().HaveCount(1);

            domainNewCharges[0].Type.Should().BeEquivalentTo(entityDetailedNewCharges[0].Type);
            domainNewCharges[0].SubType.Should().BeEquivalentTo(entityDetailedNewCharges[0].SubType);
            domainNewCharges[0].Frequency.Should().BeEquivalentTo(entityDetailedNewCharges[0].Frequency);
            domainNewCharges[0].Amount.Should().Be(entityDetailedNewCharges[0].Amount);
            domainNewCharges[0].StartDate.Should().Be(entityDetailedNewCharges[0].StartDate);
            domainNewCharges[0].EndDate.Should().Be(entityDetailedNewCharges[0].EndDate);
        }
    }
}
