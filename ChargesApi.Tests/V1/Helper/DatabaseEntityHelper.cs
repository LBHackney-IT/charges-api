using AutoFixture;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Infrastructure.Entities;

namespace ChargeApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static ChargeDbEntity CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<Charge>();

            return CreateDatabaseEntityFrom(entity);
        }

        public static ChargeDbEntity CreateDatabaseEntityFrom(Charge charge)
        {
            return new ChargeDbEntity
            {
                Id = charge.Id,
                DetailedCharges = charge.DetailedCharges,
                TargetId = charge.TargetId,
                TargetType= charge.TargetType
            };
        }
    }
}
