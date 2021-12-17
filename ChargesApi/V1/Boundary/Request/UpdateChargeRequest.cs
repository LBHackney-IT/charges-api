using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure;
using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Boundary.Request
{
    public class UpdateChargeRequest
    {
        [NonEmptyGuid]
        public Guid Id { get; set; }

        [NonEmptyGuid]
        public Guid TargetId { get; set; }

        [AllowedValues(typeof(TargetType))]
        public TargetType TargetType { get; set; }

        [AllowedValues(typeof(ChargeGroup))]
        public ChargeGroup ChargeGroup { get; set; }

        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }
    }
}
