using ChargesApi.V1.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChargesApi.V1.Domain
{
    public class DetailedCharges
    {
        /// <example>Service</example>
        [Required]
        public string Type { get; set; }

        /// <example>Estates Cleaning</example>
        [Required]
        public string SubType { get; set; }

        /// <example>Estate</example>
        public ChargeType ChargeType { get; set; }

        /// <example>DCE</example>
        public string ChargeCode { get; set; }

        /// <example>Weekly</example>
        [Required]
        public string Frequency { get; set; }

        /// <example>50</example>
        [Range(0, (double) decimal.MaxValue, ErrorMessage = "The amount value is wrong")]
        public decimal Amount { get; set; }

        /// <example>2022-02-09T12:41:15.583Z</example>
        [RequiredDateTime]
        public DateTime StartDate { get; set; }

        /// <example>2022-02-09T12:41:15.583Z</example>
        [RequiredDateTime]
        public DateTime EndDate { get; set; }
    }
}
