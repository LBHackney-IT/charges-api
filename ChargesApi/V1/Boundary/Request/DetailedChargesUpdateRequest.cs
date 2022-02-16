using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure;

namespace ChargesApi.V1.Boundary.Request
{
    public class DetailedChargesUpdateRequest
    {
        /// <example>Estates Cleaning</example>
        [Required]
        public string SubType { get; set; }

        /// <example>Estate</example>
        public ChargeType ChargeType { get; set; }

        /// <example>50</example>
        [Range(0, (double) decimal.MaxValue, ErrorMessage = "The amount value is wrong")]
        public decimal Amount { get; set; }
    }
}
