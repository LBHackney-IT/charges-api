using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Infrastructure;
using ChargesApi.V1.Infrastructure.Entities;
using ChargesApi.V1.Infrastructure.JWT;
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

        public async Task AddAsync(ChargesList chargesList, string token)
        {
            var databaseModel = chargesList.ToDatabase();
            databaseModel.CreatedAt = DateTime.UtcNow;
            databaseModel.CreatedBy = Helper.GetUserName(token); ;
            await _dynamoDbContext.SaveAsync(chargesList.ToDatabase()).ConfigureAwait(false);
        }

        public async Task<List<ChargesList>> GetAllChargesListAsync(string chargeCode)
        {
            var request = new QueryRequest
            {
                TableName = "ChargesList",
                KeyConditionExpression = "charge_code = :V_charge_code",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":V_charge_code",new AttributeValue{S = chargeCode.ToString()}},
                },
                ScanIndexForward = true
            };

            var chargesLists = await _amazonDynamoDb.QueryAsync(request).ConfigureAwait(false);

            return chargesLists?.ToChargesListDomain();

        }

        public async Task<ChargesList> GetChargesListByIdAsync(Guid id, string chargeCode)
        {
            var chargesList = await _dynamoDbContext.LoadAsync<ChargesListDbEntity>(chargeCode, id).ConfigureAwait(false);
            return chargesList?.ToDomain();
        }
    }
}
