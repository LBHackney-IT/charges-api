using ChargesApi.Tests.V1.Helper;
using ChargesApi.V1.Domain;
using FluentAssertions;
using Xunit;

namespace ChargesApi.Tests.V1.Domain
{
    public class ChargesListTests
    {
        [Fact]
        public void ChargesListHasPropertiesSet()
        {
            ChargesList chargeList = Constants.ConstructChargeListFromConstants();

            chargeList.Id.Should().Be(Constants.ID);
            chargeList.ChargeType.Should().Be(Constants.CHARGETYPE);
            chargeList.ChargeGroup.Should().Be(Constants.CHARGEGROUP);
            chargeList.ChargeName.Should().Be(Constants.CHARGENAME);
            chargeList.ChargeCode.Should().Be(Constants.CHARGECODE);
        }
    }
}
