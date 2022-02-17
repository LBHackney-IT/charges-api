using AutoFixture;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.Entities;

namespace ChargesApi.Tests.V1.Helper
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
                VersionId = charge.VersionId,
                TargetType = charge.TargetType
            };
        }
    }
}
