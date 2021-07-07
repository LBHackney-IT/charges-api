using ChargeApi.V1.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChargeApi.V1.Domain
{
    public class DetailedCharges
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public string SubType { get; set; }

        [Required]
        public string Frequency { get; set; }

        [Range(0, (double) decimal.MaxValue, ErrorMessage = "The amount value is wrong")]
        public decimal Amount { get; set; }

        [RequiredDateTime]
        public DateTime StartDate { get; set; }

        [RequiredDateTime]
        public DateTime EndDate { get; set; }
    }
}
