using ChargeApi.Tests.V1.Helper;
using ChargeApi.V1.Domain;
using FluentAssertions;
using System;
using Xunit;

namespace ChargeApi.Tests.V1.Domain
{
    public class DetailedChargesTest
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

        //[Theory]
        //[InlineData(0)]
        //[InlineData(-100)]
        //[InlineData(100)]
        //public void Amountlessthan1ThrowException(decimal value)
        //{
        //    DetailedCharges detailedCharges = new DetailedCharges();

        //    if (value <= 0)
        //        Assert.Throws<ArgumentOutOfRangeException>(() => detailedCharges.Amount = value);
        //    else
        //    {
        //        detailedCharges.Amount = value;
        //        Assert.True(detailedCharges.Amount == value);
        //    }
        //}

    }
}
