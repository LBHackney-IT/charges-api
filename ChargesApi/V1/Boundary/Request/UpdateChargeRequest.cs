using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChargesApi.V1.Boundary.Request
{
    public class UpdateChargeRequest
    {
        [NonEmptyGuid]
        public Guid Id { get; set; }

        [NonEmptyGuid]
        public Guid TargetId { get; set; }

        [AllowedValues(TargetType.Asset, TargetType.Tenure)]
        public TargetType TargetType { get; set; }

        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }
    }
}
