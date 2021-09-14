using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Infrastructure;
using ChargesApi.V1.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways
{
    public class ChargesListApiGateway : IChargesListApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly IAmazonDynamoDB _amazonDynamoDb;

        public ChargesListApiGateway(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB amazonDynamoDb)
        {
            _dynamoDbContext = dynamoDbContext;
            _amazonDynamoDb = amazonDynamoDb;
        }

        public async Task AddAsync(ChargesList chargesList)
        {
            await _dynamoDbContext.SaveAsync(chargesList.ToDatabase()).ConfigureAwait(false);
        }

        public async Task<List<ChargesList>> GetAllChargesListAsync(string chargeGroup, string chargeType)
        {
            var request = new QueryRequest
            {
                TableName = "ChargesList",
                IndexName = "charge_type_dx",
                KeyConditionExpression = "charge_type = :V_charge_type",
                FilterExpression = "charge_group = :V_charge_group",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":V_charge_type",new AttributeValue{S = chargeType.ToString()}},
                    {":V_charge_group",new AttributeValue{S = chargeGroup.ToString()}}
                },
                ScanIndexForward = true
            };

            var chargesLists = await _amazonDynamoDb.QueryAsync(request).ConfigureAwait(false);

            return chargesLists.ToChargesListDomain();

        }

        public async Task<ChargesList> GetChargesListByIdAsync(Guid id)
        {
            var chargesList = await _dynamoDbContext.LoadAsync<ChargesListDbEntity>(id).ConfigureAwait(false);
            return chargesList?.ToDomain();
        }
    }
}
