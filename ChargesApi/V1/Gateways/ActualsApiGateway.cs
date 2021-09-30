using Amazon.DynamoDBv2.DataModel;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Infrastructure.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways
{
    public class ActualsApiGateway : IActualsApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public ActualsApiGateway(IDynamoDBContext dynamoDBContext)
        {
            _dynamoDbContext = dynamoDBContext;
        }
        public async Task<bool> SaveActualsBatch(List<Actual> actuals)
        {
            var estimateBatch = _dynamoDbContext.CreateBatchWrite<ActualsDbEntity>();

            var items = actuals.ToDatabase();
            int maxBatchCount = 1000;
            if (items.Count > maxBatchCount)
            {
                var loopCount = (items.Count / maxBatchCount) + 1;
                for (int start = 0; start < loopCount; start++)
                {
                    var itemsToWrite = items.Skip(start * maxBatchCount).Take(maxBatchCount);
                    estimateBatch.AddPutItems(itemsToWrite);
                    await estimateBatch.ExecuteAsync().ConfigureAwait(false);
                }
            }
            else
            {
                estimateBatch.AddPutItems(items);
                await estimateBatch.ExecuteAsync().ConfigureAwait(false);
            }
            return true;
        }
    }
}
