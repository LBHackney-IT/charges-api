using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ChargesApi.Tests.V1.Factories
{
    public class ResponseFactoryTests
    {
        [Fact]
        public void CanMapAChargeDomainObjectToResponse()
        {
            var domain = new Charge
            {
                Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
                TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                TargetType = TargetType.asset,
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

            var response = domain.ToResponse();

            response.Id.Should().Be(new Guid("0f668265-1501-4722-8e37-77c7116dae2f"));
            response.TargetId.Should().Be(new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"));
            response.TargetType.Should().Be(TargetType.asset);
            response.DetailedCharges.Should().NotBeNull();
            response.DetailedCharges.Should().HaveCount(1);

            var detailedCharges = response.DetailedCharges.ToList();

            detailedCharges[0].Type.Should().BeEquivalentTo("Type");
            detailedCharges[0].SubType.Should().BeEquivalentTo("SubType");
            detailedCharges[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            detailedCharges[0].EndDate.Should().Be(new DateTime(2021, 7, 4));
            detailedCharges[0].Amount.Should().Be(150);
            detailedCharges[0].Frequency.Should().BeEquivalentTo("Frequency");
        }
        [Fact]
        public void CanMapAChargeMaintenanceDomainObjectToResponse()
        {
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
                Status = ChargeMaintenanceStatus.pending
            };

            var response = domain.ToResponse();

            response.Id.Should().Be(new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"));
            response.ChargesId.Should().Be(new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"));
            response.Status.Should().Be(ChargeMaintenanceStatus.pending);
            response.Reason.Should().Be("Uplift");
            response.StartDate.Should().Be(new DateTime(2021, 7, 2));
            response.ExistingValue.Should().HaveCount(1);

            var existingCharges = response.ExistingValue.ToList();

            existingCharges[0].Type.Should().BeEquivalentTo("service");
            existingCharges[0].SubType.Should().BeEquivalentTo("water");
            existingCharges[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            existingCharges[0].EndDate.Should().Be(new DateTime(2021, 7, 4));
            existingCharges[0].Amount.Should().Be(120);
            existingCharges[0].Frequency.Should().BeEquivalentTo("weekly");

            var newCharges = response.NewValue.ToList();

            newCharges[0].Type.Should().BeEquivalentTo("service");
            newCharges[0].SubType.Should().BeEquivalentTo("water");
            newCharges[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            newCharges[0].EndDate.Should().Be(new DateTime(2021, 7, 4));
            newCharges[0].Amount.Should().Be(150);
            newCharges[0].Frequency.Should().BeEquivalentTo("weekly");
        }

        [Fact]
        public void CanMapListOfAssetSummaryDomainObjectsToResponse()
        {
            var firstDomain = new Charge
            {
                Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
                TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                TargetType = TargetType.asset,
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

            var secondDomain = new Charge
            {
                Id = new Guid("762cdddf-659a-4438-b67e-ee78c702a8d9"),
                TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                TargetType = TargetType.asset,
                DetailedCharges = new List<DetailedCharges>()
                {
                    new DetailedCharges
                    {
                        Type = "Type",
                        SubType = "SubType",
                        StartDate = new DateTime(2021, 7, 1),
                        EndDate = new DateTime(2021, 7, 7),
                        Amount = 250,
                        Frequency = "Frequency"
                    }
                }
            };

            var listOfCharges = new List<Charge>()
            {
                firstDomain,
                secondDomain
            };

            var response = listOfCharges.ToResponse();

            response[0].Id.Should().Be(new Guid("0f668265-1501-4722-8e37-77c7116dae2f"));
            response[0].TargetId.Should().Be(new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"));
            response[0].TargetType.Should().Be(TargetType.asset);
            response[0].DetailedCharges.Should().NotBeNull();
            response[0].DetailedCharges.Should().HaveCount(1);

            var detailedCharges = response[0].DetailedCharges.ToList();

            detailedCharges[0].Type.Should().BeEquivalentTo("Type");
            detailedCharges[0].SubType.Should().BeEquivalentTo("SubType");
            detailedCharges[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            detailedCharges[0].EndDate.Should().Be(new DateTime(2021, 7, 4));
            detailedCharges[0].Amount.Should().Be(150);
            detailedCharges[0].Frequency.Should().BeEquivalentTo("Frequency");

            response[1].Id.Should().Be(new Guid("762cdddf-659a-4438-b67e-ee78c702a8d9"));
            response[1].TargetId.Should().Be(new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"));
            response[1].TargetType.Should().Be(TargetType.asset);
            response[1].DetailedCharges.Should().NotBeNull();
            response[1].DetailedCharges.Should().HaveCount(1);

            detailedCharges = response[1].DetailedCharges.ToList();

            detailedCharges[0].Type.Should().BeEquivalentTo("Type");
            detailedCharges[0].SubType.Should().BeEquivalentTo("SubType");
            detailedCharges[0].StartDate.Should().Be(new DateTime(2021, 7, 1));
            detailedCharges[0].EndDate.Should().Be(new DateTime(2021, 7, 7));
            detailedCharges[0].Amount.Should().Be(250);
            detailedCharges[0].Frequency.Should().BeEquivalentTo("Frequency");
        }
    }
}
