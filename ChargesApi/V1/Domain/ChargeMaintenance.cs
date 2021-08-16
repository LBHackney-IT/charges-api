using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Domain
{
    public class ChargeMaintenance
    {
        public Guid Id { get; set; }
        public Guid ChargesId { get; set; }

        public IEnumerable<DetailedCharges> ExistingValue { get; set; }

        public IEnumerable<DetailedCharges> NewValue { get; set; }

        public string Reason { get; set; }

        public DateTime StartDate { get; set; }

        public ChargeMaintenanceStatus Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string LastUpdatedBy { get; set; }

        public DateTime LastUpdatedDate { get; set; }
    }
}
