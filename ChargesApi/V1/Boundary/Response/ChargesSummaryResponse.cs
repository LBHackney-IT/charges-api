using ChargesApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Boundary.Response
{
    public class ChargesSummaryResponse
    {
        public ChargesSummaryResponse()
        {
            TenantsCharges = new List<ChargeDetail>();
            LeaseholdersCharges = new List<ChargeDetail>();
        }

        public Guid TargetId { get; set; }
        public TargetType TargetType { get; set; }
        public IEnumerable<ChargeDetail> TenantsCharges { get; set; }
        public IEnumerable<ChargeDetail> LeaseholdersCharges { get; set; }
    }
}
