using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Domain
{
    public class Charge
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public TargetType TargetType { get; set; }
        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }
    }
}

