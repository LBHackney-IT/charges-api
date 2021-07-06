using ChargeApi.V1.Domain;
using ChargeApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChargeApi.V1.Boundary.Request
{
    public class AddChargeRequest
    {
        [Required]
        public Guid TargetId { get; set; }

        [AllowedValues(TargetType.Asset, TargetType.Tenure)]
        public TargetType TargetType { get; set; }

        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }
    }
}
