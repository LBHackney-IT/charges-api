using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChargeApi.V1.Domain
{
    public class DetailedCharges
    {
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Frequency { get; set; }
        [Range(0,double.MaxValue,ErrorMessage = "The amount value is wrong")]
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
