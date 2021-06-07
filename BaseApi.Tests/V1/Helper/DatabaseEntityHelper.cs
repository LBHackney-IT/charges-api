using AutoFixture;
using BaseApi.V1.Domain;
using BaseApi.V1.Infrastructure;

namespace BaseApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static ChargeDbEntity CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<Entity>();

            return CreateDatabaseEntityFrom(entity);
        }

        public static ChargeDbEntity CreateDatabaseEntityFrom(Entity entity)
        {
            return new ChargeDbEntity
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
            };
        }
    }
}
