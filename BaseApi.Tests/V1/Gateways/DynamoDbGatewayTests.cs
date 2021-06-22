using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using ChargeApi.Tests.V1.Helper;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.Infrastructure;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ChargeApi.Tests.V1.Gateways
{
    //TODO: Remove this file if DynamoDb gateway not being used
    //TODO: Rename Tests to match gateway name
    //For instruction on how to run tests please see the wiki: https://github.com/LBHackney-IT/lbh-base-api/wiki/Running-the-test-suite.

    public class DynamoDbGatewayTests
    {
        //private readonly Fixture _fixture = new Fixture();
        //private Mock<IDynamoDBContext> _dynamoDb;
        //private DynamoDbGateway _classUnderTest;

        //public DynamoDbGatewayTests()
        //{
        //    _dynamoDb = new Mock<IDynamoDBContext>();
        //    _classUnderTest = new DynamoDbGateway(_dynamoDb.Object);
        //}

        //[Fact]
        //public void GetEntityByIdReturnsNullIfEntityDoesntExist()
        //{
        //    var id = Guid.NewGuid();
        //    var response = _classUnderTest.GetChargeByIdAsync(id);

        //    response.Should().BeNull();
        //}

        //[Fact]
        //public async Task GetEntityByIdReturnsTheEntityIfItExists()
        //{
        //    // Arrange
        //    var id = Guid.NewGuid();
        //    var charge = _fixture.Build<Charge>()
        //                    .With(x => x.Id, id)
        //                    .Create();
        //    charge.DetailedCharges = new[] { charge.DetailedCharges.First() };
        //    charge.DetailedCharges.First().Amount = 123;

        //    var chargedbEntity = charge.ToDatabase();
        //    await _dynamoDb

        //}
    }
}
