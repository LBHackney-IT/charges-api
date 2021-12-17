using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChargesApi.V1.Boundary.Request
{
    public class AddChargesUpdateRequest
    {
        /// <summary>
        /// Type of Charge Group [Tenants, Leaseholders]
        /// </summary>
        /// <example>
        /// Tenants
        /// </example>
        [Required]
        [AllowedValues(typeof(ChargeGroup))]
        public ChargeGroup ChargeGroup { get; set; }
        /// <summary>
        /// Type of Charge [Estate, Block, Property]
        /// </summary>
        /// <example>
        /// Estate
        /// </example>
        [Required]
        [AllowedValues(typeof(ChargeType))]
        public ChargeType ChargeType { get; set; }

        public Guid TargetId { get; set; }

        [Required]
        public IEnumerable<ChargeItem> ChargesItems { get; set; }
    }
}
