using ChargeApi.V1.Domain;
using ChargeApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChargeApi.V1.Boundary.Request
{
    public class AddChargeMaintenanceRequest
    {
        [NonEmptyGuid]
        public Guid ChargesId { get; set; }

        [Required]
        public IEnumerable<DetailedCharges> ExistingValue { get; set; }

        [Required]
        public IEnumerable<DetailedCharges> NewValue { get; set; }

        [Required]
        public string Reason { get; set; }

        [RequiredDateTime]
        public DateTime StartDate { get; set; }

        [AllowedMaintenanceStatus(ChargeMaintenanceStatus.Applied, ChargeMaintenanceStatus.Pending)]
        public ChargeMaintenanceStatus Status { get; set; }
    }
}
