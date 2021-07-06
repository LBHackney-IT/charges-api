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

        [Range(double.Epsilon, (double) decimal.MaxValue, ErrorMessage = "Please enter the valid Amount.")]
        public decimal Amount { get; set; }

        [RequiredDateTime]
        public DateTime StartDate { get; set; }

        [RequiredDateTime]
        public DateTime EndDate { get; set; }
    }
}
