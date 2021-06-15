using ChargeApi.V1.Domain;
using ChargeApi.V1.Infrastructure;

namespace ChargeApi.V1.Factories
{
    public static class EntityFactory
    {
        public static Charge ToDomain(this ChargeDbEntity databaseEntity)
        {
            return new Charge
            {
                Id = databaseEntity.Id,
                TargetId = databaseEntity.TargetId,
                TargetType = databaseEntity.TargetType,
                ChargeDetails = databaseEntity.ChargeDetails                
            };
        }

        public static ChargeDbEntity ToDatabase(this Charge entity)
        {
            return new ChargeDbEntity
            {
                Id = entity.Id,
                TargetId = entity.TargetId,
                TargetType = entity.TargetType,
                ChargeDetails = entity.ChargeDetails
            };
        }
    }
}
