using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;

namespace ChargesApi.V1.Domain
{
    public class ChargesUpdateDomain
    {
        public ChargeSubGroup? ChargeSubGroup { get; set; }
        public short ChargeYear { get; set; }
        public IEnumerable<DetailedChargesUpdateRequest> DetailedCharges { get; set; }
    }
}
