using ChargesApi.Tests.V1.Helper;
using ChargesApi.V1.Domain;
using FluentAssertions;
using Xunit;

namespace ChargesApi.Tests.V1.Domain
{
    public class ChargeItemTests
    {
        [Fact]
        public void ChargesItemHasPropertiesSet()
        {
            ChargeItem chargeItem = Constants.ConstructChargeItemFromConstants();

            chargeItem.IsChargeApplicable.Should().BeTrue();
            chargeItem.PerPropertyCharge.Should().Be(Constants.AMOUNT);
            chargeItem.ChargeName.Should().Be(Constants.CHARGENAME);
            chargeItem.ChargeCode.Should().Be(Constants.CHARGECODE);
        }
    }
}
