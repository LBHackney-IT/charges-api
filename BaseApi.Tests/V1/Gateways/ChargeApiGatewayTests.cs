using AutoFixture;
using ChargeApi.Tests.V1.Helper;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Gateways;
using FluentAssertions;
using Xunit;

namespace ChargeApi.Tests.V1.Gateways
{
    //TODO: Remove this file if Postgres gateway is not being used
    //TODO: Rename Tests to match gateway name
    //For instruction on how to run tests please see the wiki: https://github.com/LBHackney-IT/lbh-base-api/wiki/Running-the-test-suite.
    
    public class ChargeApiGatewayTests ///////////: DatabaseTests
    {
        //private readonly Fixture _fixture = new Fixture();
        //private ChargeApiGateway _classUnderTest;

        //public ChargeApiGatewayTests()
        //{
        //    _classUnderTest = new ChargeApiGateway(DatabaseContext);
        //}

        //[Fact]
        //public void GetEntityByIdReturnsNullIfEntityDoesntExist()
        //{
        //    var response = _classUnderTest.GetEntityById(123);

        //    response.Should().BeNull();
        //}

        //[Fact]
        //public void GetEntityByIdReturnsTheEntityIfItExists()
        //{
        //    var entity = _fixture.Create<Entity>();
        //    var databaseEntity = DatabaseEntityHelper.CreateDatabaseEntityFrom(entity);

        //    DatabaseContext.DatabaseEntities.Add(databaseEntity);
        //    DatabaseContext.SaveChanges();

        //    var response = _classUnderTest.GetChargeById(databaseEntity.Id);

        //    databaseEntity.Id.Should().Be(response.Id);
        //    databaseEntity.CreatedAt.Should().BeSameDateAs(response.CreatedAt);
        //} 
    }
}
