using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.Domain
{
    public class DetailedChargesUpdateDomain
    {
        public string SubType { get; set; }

        public ChargeType ChargeType { get; set; }

        public decimal DifferenceAmount { get; set; }
    }
}
