using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargeApi.V1.Domain
{
    public class ChargeDetails
    {
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Frequency { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
