using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.Domain
{
    public class ChargeItem
    {
        public string ChargeName { get; set; }

        public string ChargeCode { get; set; }

        public decimal PerPropertyCharge { get; set; }

        public bool IsChargeApplicable { get; set; }
    }
}
