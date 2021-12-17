using Amazon.DynamoDBv2.Model;
using AutoFixture;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChargesApi.Tests.V1.Helper
{
    public static class FakeDataHelper
    {
        private static readonly Fixture _fixture = new Fixture();

        public static QueryResponse MockQueryResponse<T>()
        {
            QueryResponse response = new QueryResponse();
            if (typeof(T) == typeof(ChargesListResponse))
            {
                response.Items.Add(
                new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue { S = _fixture.Create<Guid>().ToString() } },
                        { "charge_name", new AttributeValue { S = _fixture.Create<string>().ToString() } },
                        { "charge_code", new AttributeValue { S = _fixture.Create<string>() } },
                        { "charge_type", new AttributeValue { S = _fixture.Create<ChargeType>().ToString() } },
                        { "charge_group", new AttributeValue { S = _fixture.Create<ChargeGroup>().ToString() } },
                        { "created_by", new AttributeValue { S = _fixture.Create<string>() } },
                        { "last_updated_by", new AttributeValue { S = _fixture.Create<string>() } },
                        { "created_date", new AttributeValue { S = _fixture.Create<DateTime>().ToString("F") } },
                        { "last_updated_date", new AttributeValue { S = _fixture.Create<DateTime>().ToString("F") } }
                    });
            }
            if (typeof(T) == typeof(Charge))
            {
                response.Items.Add(
                new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue { S = _fixture.Create<Guid>().ToString() } },
                        { "target_id", new AttributeValue { S = _fixture.Create<Guid>().ToString() } },
                        { "target_type", new AttributeValue { S = _fixture.Create<TargetType>().ToString() } },
                        { "charge_group", new AttributeValue { S = _fixture.Create<ChargeGroup>().ToString() } },
                        { "created_by", new AttributeValue { S = _fixture.Create<string>() } },
                        { "last_updated_by", new AttributeValue { S = _fixture.Create<string>() } },
                        { "created_date", new AttributeValue { S = _fixture.Create<DateTime>().ToString("F") } },
                        { "last_updated_date", new AttributeValue { S = _fixture.Create<DateTime>().ToString("F") } },
                        {
                            "detailed_charges", new AttributeValue
                            {
                                L = Enumerable.Range(0, new Random(10).Next(1, 10))
                                    .Select(p =>
                                        new AttributeValue
                                        {
                                            M =
                                            {
                                                {
                                                    "amount",
                                                    new AttributeValue
                                                    {
                                                        N = _fixture.Create<decimal>().ToString("F")
                                                    }
                                                },
                                                {
                                                    "frequency",
                                                    new AttributeValue { S = _fixture.Create<string>() }
                                                },
                                                { "type", new AttributeValue { S = _fixture.Create<string>() } },
                                                { "subType", new AttributeValue { S = _fixture.Create<string>() } },
                                                { "chargeCode", new AttributeValue { S = _fixture.Create<string>() } },
                                                { "chargeType", new AttributeValue { S = _fixture.Create<ChargeType>().ToString() } },
                                                { "startDate", new AttributeValue { S = _fixture.Create<DateTime>().ToString("F") } },
                                                { "endDate", new AttributeValue { S = _fixture.Create<DateTime>().ToString("F") } }
                                            }
                                        }
                                    ).ToList()
                            }
                        }
                    });
            }
            return response;
        }
    }
}
