using ChargesApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Boundary.Response
{
    public class ChargesSummaryResponse
    {
        public ChargesSummaryResponse()
        {
            ChargesList = new List<ChargeDetail>();
        }

        public Guid TargetId { get; set; }
        public TargetType TargetType { get; set; }
        public IEnumerable<ChargeDetail> ChargesList { get; set; }
    }
}
