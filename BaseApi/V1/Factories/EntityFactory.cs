using BaseApi.V1.Domain;
using BaseApi.V1.Infrastructure;

namespace BaseApi.V1.Factories
{
    public static class EntityFactory
    {
        public static Charge ToDomain(this ChargeDbEntity databaseEntity)
        {
            return new Charge
            {
                Id = databaseEntity.Id,
                TargetId = databaseEntity.TargetId,
                ChargeType = databaseEntity.ChargeType,
                DataImportSource = databaseEntity.DataImportSource,
                DebitActive = databaseEntity.DebitActive,
                DebitCode = databaseEntity.DebitCode,
                DebitCodeDescription = databaseEntity.DebitCodeDescription,
                DebitLastCharged = databaseEntity.DebitLastCharged,
                DebitNextDue = databaseEntity.DebitNextDue,
                DebitSource = databaseEntity.DebitSource,
                DebitValue = databaseEntity.DebitValue,
                EffectiveStartDate = databaseEntity.EffectiveStartDate,
                PeriodCode = databaseEntity.PeriodCode,
                PropertyDebit = databaseEntity.PropertyDebit,
                ServiceChargeSchedule = databaseEntity.ServiceChargeSchedule,
                TerminationDate = databaseEntity.TerminationDate,
                TimeStamp = databaseEntity.TimeStamp,
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
                ChargeType = entity.ChargeType,
                TimeStamp = entity.TimeStamp,
                TerminationDate = entity.TerminationDate,
                ServiceChargeSchedule = entity.ServiceChargeSchedule,
                PropertyDebit = entity.PropertyDebit,
                PeriodCode = entity.PeriodCode,
                EffectiveStartDate = entity.EffectiveStartDate,
                DebitValue = entity.DebitValue,
                DebitSource = entity.DebitSource,
                DataImportSource = entity.DataImportSource,
                DebitActive = entity.DebitActive,
                DebitCode = entity.DebitCode,
                DebitCodeDescription = entity.DebitCodeDescription,
                DebitLastCharged = entity.DebitLastCharged,
                DebitNextDue = entity.DebitNextDue,
                TargetType = entity.TargetType,
                ChargeDetails = entity.ChargeDetails
            };
        }
    }
}
