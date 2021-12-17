using ChargesApi.Tests.V1.Helper;
using ChargesApi.V1.Domain;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace ChargesApi.Tests.V1.Domain
{
    public class ChargeMaintenanceTests
    {
        [Fact]
        public void ChargeMaintenanceHasPropertiesSet()
        {
            ChargeMaintenance chargeMaintenance = Constants.ConstructChargeMaintenanceFromConstants();

            chargeMaintenance.Id.Should().Be(Constants.ID);
            chargeMaintenance.ChargesId.Should().Be(Constants.TARGETID);
            chargeMaintenance.Reason.Should().Be("Uplift");
            chargeMaintenance.NewValue.Should().ContainSingle();
            chargeMaintenance.NewValue.First().Amount.Should().Be(Constants.AMOUNT);
            chargeMaintenance.NewValue.First().StartDate.Should().Be(DateTime.Parse(Constants.STARTDATE));
            chargeMaintenance.NewValue.First().EndDate.Should().Be(DateTime.Parse(Constants.ENDDATE));
            chargeMaintenance.NewValue.First().Frequency.Should().Be(Constants.FREQUENCY);
            chargeMaintenance.NewValue.First().Type.Should().Be(Constants.TYPE);
            chargeMaintenance.NewValue.First().SubType.Should().Be(Constants.SUBTYPE);

            chargeMaintenance.ExistingValue.Should().ContainSingle();
            chargeMaintenance.ExistingValue.First().Amount.Should().Be(Constants.AMOUNT);
            chargeMaintenance.ExistingValue.First().StartDate.Should().Be(DateTime.Parse(Constants.STARTDATE));
            chargeMaintenance.ExistingValue.First().EndDate.Should().Be(DateTime.Parse(Constants.ENDDATE));
            chargeMaintenance.ExistingValue.First().Frequency.Should().Be(Constants.FREQUENCY);
            chargeMaintenance.ExistingValue.First().Type.Should().Be(Constants.TYPE);
            chargeMaintenance.ExistingValue.First().SubType.Should().Be(Constants.SUBTYPE);
        }
    }
}
