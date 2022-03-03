using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure;

namespace ChargesApi.V1.Boundary.Request
{
    public class PropertyChargesQueryParameters
    {
        public short ChargeYear { get; set; }

        [AllowedValues(typeof(ChargeGroup))]
        public ChargeGroup ChargeGroup { get; set; }

        [AllowedValues(typeof(ChargeSubGroup))]
        public ChargeSubGroup ChargeSubGroup { get; set; }
    }
}
