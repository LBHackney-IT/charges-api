using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure;

namespace ChargesApi.V1.Boundary.Request
{
    public class ChargesUpdateRequest
    {
        [NonEmptyGuid]
        public Guid TargetId { get; set; }

        [AllowedValues(typeof(ChargeSubGroup))]
        public ChargeSubGroup? ChargeSubGroup { get; set; }

        [Required]
        public short ChargeYear { get; set; }

        public IEnumerable<DetailedChargesUpdateRequest> DetailedCharges { get; set; }
    }
}
