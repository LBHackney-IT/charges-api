using ChargesApi.Tests.V1.Helper;
using ChargesApi.V1.Domain;
using FluentAssertions;
using System;
using System.Linq;
using ChargesApi.V1.Infrastructure;
using Xunit;

namespace ChargesApi.Tests.V1.Domain
{
    public class ChargeTests
    {
        [Fact]
        public void ChargeHasPropertiesSet()
        {
            Charge charge = Constants.ConstructChargeFromConstants();

            charge.Id.Should().Be(Constants.ID);
            charge.TargetId.Should().Be(Constants.TARGETID);
            charge.TargetType.Should().Be(Constants.TARGETTYPE);
            charge.DetailedCharges.Should().ContainSingle();
            charge.DetailedCharges.First().Amount.Should().Be(Constants.AMOUNT);
            charge.DetailedCharges.First().StartDate.Should().Be(DateTime.Parse(Constants.STARTDATE));
            charge.DetailedCharges.First().EndDate.Should().Be(DateTime.Parse(Constants.ENDDATE));
            charge.DetailedCharges.First().Frequency.Should().Be(Constants.FREQUENCY);
            charge.DetailedCharges.First().Type.Should().Be(Constants.TYPE);
            charge.DetailedCharges.First().SubType.Should().Be(Constants.SUBTYPE);
        }
    }
}
