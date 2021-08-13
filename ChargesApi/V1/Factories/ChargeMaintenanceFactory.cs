using ChargeApi.V1.Boundary.Request;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Infrastructure.Entities;

namespace ChargeApi.V1.Factories
{
    public static class ChargeMaintenanceFactory
    {
        public static ChargeMaintenance ToDomain(this ChargesMaintenanceDbEntity chargeMaintenanceEntity)
        {
            if (chargeMaintenanceEntity == null)
            {
                return null;
            }

            return new ChargeMaintenance
            {
                Id = chargeMaintenanceEntity.Id,
                ChargesId = chargeMaintenanceEntity.ChargesId,
                Reason = chargeMaintenanceEntity.Reason,
                Status = chargeMaintenanceEntity.Status,
                StartDate = chargeMaintenanceEntity.StartDate,
                ExistingValue = chargeMaintenanceEntity.ExistingValue,
                NewValue = chargeMaintenanceEntity.NewValue
            };
        }

        public static ChargesMaintenanceDbEntity ToDatabase(this ChargeMaintenance chargeMaintenance)
        {
            if (chargeMaintenance == null)
            {
                return null;
            }

            return new ChargesMaintenanceDbEntity
            {
                Id = chargeMaintenance.Id,
                ChargesId = chargeMaintenance.ChargesId,
                Reason = chargeMaintenance.Reason,
                Status = chargeMaintenance.Status,
                StartDate = chargeMaintenance.StartDate,
                ExistingValue = chargeMaintenance.ExistingValue,
                NewValue = chargeMaintenance.NewValue
            };
        }

        public static ChargeMaintenance ToDomain(this AddChargeMaintenanceRequest chargeMaintenanceRequest)
        {
            if (chargeMaintenanceRequest == null)
            {
                return null;
            }

            return new ChargeMaintenance
            {
                ChargesId = chargeMaintenanceRequest.ChargesId,
                Reason = chargeMaintenanceRequest.Reason,
                Status = chargeMaintenanceRequest.Status,
                StartDate = chargeMaintenanceRequest.StartDate,
                ExistingValue = chargeMaintenanceRequest.ExistingValue,
                NewValue = chargeMaintenanceRequest.NewValue
            };
        }
    }
}
