using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace ChargesApi.V1.Boundary.Request
{
    public class AddChargesListRequest
    {
        [Required]
        public string ChargeName { get; set; }

        [Required]
        public string ChargeCode { get; set; }

        /// <summary>
        /// Type of Charge [estate, block, property]
        /// </summary>
        /// <example>
        /// Estate
        /// </example>
        [Required]
        [AllowedValues(typeof(ChargeType))]
        public ChargeType ChargeType { get; set; }

        /// <summary>
        /// Type of Charge Group [tenants, leaseholders]
        /// </summary>
        /// <example>
        /// Tenants
        /// </example>
        [Required]
        [AllowedValues(typeof(ChargeGroup))]
        public ChargeGroup ChargeGroup { get; set; }
    }
}
