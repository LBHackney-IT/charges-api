using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure;
using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Boundary.Request
{
    public class AddChargeRequest
    {
        [NonEmptyGuid]
        public Guid TargetId { get; set; }

        [AllowedValues(TargetType.Asset, TargetType.Tenure)]
        public TargetType TargetType { get; set; }

        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }
    }
}
