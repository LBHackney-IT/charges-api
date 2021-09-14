using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using FluentAssertions;
using System;
using Xunit;

namespace ChargesApi.Tests.V1.Factories
{
    public class ChargesSummaryFactoryTests
    {
        [Fact]
        public void CanMapADomainEntityToAResponseObject()
        {
            var domainEntity = new DetailedCharges
            {
                Type = "Type",
                SubType = "Block Cleaning",
                StartDate = new DateTime(2021, 7, 2),
                EndDate = new DateTime(2021, 7, 4),
                Amount = 150,
                Frequency = "Frequency",
                ChargeCode = "DCB",
                ChargeType = ChargeType.block
            };

            var response = domainEntity.ToResponse();

            domainEntity.ChargeCode.Should().Be(response.ChargeCode);
            domainEntity.Amount.Should().Be(response.ChargeAmount);
            domainEntity.StartDate.Year.Should().Be(response.ChargeYear);
            domainEntity.SubType.Should().Be(response.ChargeName);
        }
    }
}
