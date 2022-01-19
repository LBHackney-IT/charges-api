using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Infrastructure;
using ChargesApi.V1.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ChargesApi.V1.Gateways
{
    public class DynamoDbGateway : IChargesApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly IConfiguration _configuration;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB amazonDynamoDb, IConfiguration configuration)
        {
            _dynamoDbContext = dynamoDbContext;
            _amazonDynamoDb = amazonDynamoDb;
            _configuration = configuration;
        }

        public void Add(Charge charge)
        {
            _dynamoDbContext.SaveAsync(charge.ToDatabase());
        }

        public async Task AddAsync(Charge charge)
        {
            var databaseModel = charge.ToDatabase();
            databaseModel.CreatedAt = DateTime.UtcNow;
            databaseModel.CreatedBy = "test";
            await _dynamoDbContext.SaveAsync(databaseModel).ConfigureAwait(false);
        }

        public void AddRange(List<Charge> charges)
        {
            charges.ForEach(e =>
            {
                _dynamoDbContext.SaveAsync(e.ToDatabase());
            });
        }

        public async Task AddRangeAsync(List<Charge> charges)
        {
            foreach (Charge charge in charges)
            {
                await _dynamoDbContext.SaveAsync(charge.ToDatabase()).ConfigureAwait(false);
            }
        }

        public async Task<List<Charge>> GetAllChargesAsync(Guid targetId)
        {
            var request = new QueryRequest
            {
                TableName = "Charges",
                KeyConditionExpression = "target_id = :V_target_id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":V_target_id",new AttributeValue{S = targetId.ToString()}}
                },
                ScanIndexForward = true
            };

            var chargesLists = await _amazonDynamoDb.QueryAsync(request).ConfigureAwait(false);

            return chargesLists?.ToChargeDomain();
        }

        public async Task<Charge> GetChargeByIdAsync(Guid id, Guid targetId)
        {
            var result = await _dynamoDbContext.LoadAsync<ChargeDbEntity>(targetId, id).ConfigureAwait(false);

            return result?.ToDomain();
        }

        public void Remove(Charge charge)
        {
            _dynamoDbContext.DeleteAsync(charge.ToDatabase());
        }

        public async Task RemoveAsync(Charge charge)
        {
            await _dynamoDbContext.DeleteAsync(charge.ToDatabase()).ConfigureAwait(false);
        }

        public void RemoveRange(List<Charge> charges)
        {
            charges.ForEach(c =>
            {
                Remove(c);
            });
        }

        public async Task RemoveRangeAsync(List<Charge> charges)
        {
            foreach (Charge c in charges)
            {
                await RemoveAsync(c).ConfigureAwait(false);
            }
        }

        public void Update(Charge charge)
        {
            _dynamoDbContext.SaveAsync(charge.ToDatabase());
        }

        public async Task UpdateAsync(Charge charge)
        {
            await _dynamoDbContext.SaveAsync(charge.ToDatabase()).ConfigureAwait(false);
        }

        public async Task<bool> AddBatchAsync(List<Charge> charges)
        {
            var chargesBatch = _dynamoDbContext.CreateBatchWrite<ChargeDbEntity>();

            var items = charges.ToDatabaseList();
            var maxBatchCount = _configuration.GetValue<int>("BatchProcessing:PerBatchCount");
            if (items.Count > maxBatchCount)
            {
                var loopCount = (items.Count / maxBatchCount) + 1;
                for (var start = 0; start < loopCount; start++)
                {
                    var itemsToWrite = items.Skip(start * maxBatchCount).Take(maxBatchCount);
                    chargesBatch.AddPutItems(itemsToWrite);
                    await chargesBatch.ExecuteAsync().ConfigureAwait(false);
                }
            }
            else
            {
                chargesBatch.AddPutItems(items);
                await chargesBatch.ExecuteAsync().ConfigureAwait(false);
            }

            return true;
        }
    }
}
