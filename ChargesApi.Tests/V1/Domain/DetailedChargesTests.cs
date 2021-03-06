using ChargesApi.Tests.V1.Helper;
using ChargesApi.V1.Domain;
using FluentAssertions;
using System;
using Xunit;

namespace ChargesApi.Tests.V1.Domain
{
    public class DetailedChargesTests
    {
        [Fact]
        public void DetailedChargesTestHasPropertiesSet()
        {
            DetailedCharges detailedCharges = Constants.ConstructDetailedChargesFromConstants();

            detailedCharges.Amount.Should().Be(Constants.AMOUNT);
            detailedCharges.StartDate.Should().Be(DateTime.Parse(Constants.STARTDATE));
            detailedCharges.EndDate.Should().Be(DateTime.Parse(Constants.ENDDATE));
            detailedCharges.SubType.Should().Be(Constants.SUBTYPE);
            detailedCharges.Type.Should().Be(Constants.TYPE);
            detailedCharges.Frequency.Should().Be(Constants.FREQUENCY);
        }
    }
}
