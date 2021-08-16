using ChargeApi.V1.Boundary.Request;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Infrastructure.Entities;

namespace ChargeApi.V1.Factories
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
                DetailedCharges = chargeEntity.DetailedCharges
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
                DetailedCharges = charge.DetailedCharges
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
                DetailedCharges = chargeRequest.DetailedCharges
            };
        }
    }
}