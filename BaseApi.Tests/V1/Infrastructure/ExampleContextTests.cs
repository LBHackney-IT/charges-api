using System.Linq;
using ChargeApi.Tests.V1.Helper;
using Xunit;

namespace ChargeApi.Tests.V1.Infrastructure
{
    //TODO: Remove this file if Postgres is not being used
    
    public class DatabaseContextTest ////////////: DatabaseTests
    {
        /*[Fact]
        public void CanGetADatabaseEntity()
        {
            var databaseEntity = DatabaseEntityHelper.CreateDatabaseEntity();

            DatabaseContext.Add(databaseEntity);
            DatabaseContext.SaveChanges();

            var result = DatabaseContext.DatabaseEntities.ToList().FirstOrDefault();

            Assert.AreEqual(result, databaseEntity);
        }*/
    }
}
