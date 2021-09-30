using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.Entities;

namespace ChargesApi.V1.Factories
{
    public static class ChargeFactory
    {
        public static Charge ToDomain(this ChargeDbEntity chargeEntity)
        {
            if (chargeEntity == null)
            {
                return null;
            }

            return new Charge
            {
                Id = chargeEntity.Id,
                TargetId = chargeEntity.TargetId,
                TargetType = chargeEntity.TargetType,
                ChargeGroup = chargeEntity.ChargeGroup,
                DetailedCharges = chargeEntity.DetailedCharges,
                CreatedBy = chargeEntity.CreatedBy,
                CreatedDate = chargeEntity.CreatedDate,
                LastUpdatedBy = chargeEntity.LastUpdatedBy,
                LastUpdatedDate = chargeEntity.LastUpdatedDate
            };
        }

        public static ChargeDbEntity ToDatabase(this Charge charge)
        {
            if (charge == null)
            {
                return null;
            }

            return new ChargeDbEntity
            {
                Id = charge.Id,
                TargetId = charge.TargetId,
                TargetType = charge.TargetType,
                ChargeGroup = charge.ChargeGroup,
                DetailedCharges = charge.DetailedCharges,
                CreatedBy = charge.CreatedBy,
                CreatedDate = charge.CreatedDate,
                LastUpdatedBy = charge.LastUpdatedBy,
                LastUpdatedDate = charge.LastUpdatedDate
            };
        }

        public static Charge ToDomain(this AddChargeRequest chargeRequest)
        {
            if (chargeRequest == null)
            {
                return null;
            }

            return new Charge
            {
                TargetId = chargeRequest.TargetId,
                TargetType = chargeRequest.TargetType,
                ChargeGroup = chargeRequest.ChargeGroup,
                DetailedCharges = chargeRequest.DetailedCharges
            };
        }

        public static Charge ToDomain(this UpdateChargeRequest chargeRequest)
        {
            if (chargeRequest == null)
            {
                return null;
            }

            return new Charge
            {
                Id = chargeRequest.Id,
                TargetId = chargeRequest.TargetId,
                TargetType = chargeRequest.TargetType,
                ChargeGroup = chargeRequest.ChargeGroup,
                DetailedCharges = chargeRequest.DetailedCharges
            };
        }
    }
}
