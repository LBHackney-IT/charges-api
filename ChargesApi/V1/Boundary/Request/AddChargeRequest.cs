using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChargesApi.V1.Boundary.Request
{
    public class AddChargeRequest
    {
        /// <example>12ad9bb5-9805-4336-a90e-8a5e34048a39</example>
        [NonEmptyGuid]
        public Guid TargetId { get; set; }

        /// <example>Block</example>
        [AllowedValues(typeof(TargetType))]
        public TargetType TargetType { get; set; }

        /// <example>Leaseholders</example>
        [AllowedValues(typeof(ChargeGroup))]
        public ChargeGroup ChargeGroup { get; set; }

        /// <summary>
        /// ChargeSubGroup is required only for Leaseholders Charge Group.  
        /// Allowed values - Estimate/Actual
        /// </summary>
        /// <example>Actual</example>
        public ChargeSubGroup? ChargeSubGroup { get; set; }

        /// <example>2022</example>
        [Required]
        public short ChargeYear { get; set; }

        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }
    }
}
