using ChargeApi.V1.Domain;
using ChargeApi.V1.Infrastructure;

namespace ChargeApi.V1.Factories
{
    public static class ChargeFactory
    {
        public static Charge ToDomain(this ChargeDbEntity chargeEntity)
        {
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
            return new ChargeDbEntity
            {
                Id = charge.Id,
                TargetId = charge.TargetId,
                TargetType = charge.TargetType,
                DetailedCharges = charge.DetailedCharges
            };
        }
    }
}
