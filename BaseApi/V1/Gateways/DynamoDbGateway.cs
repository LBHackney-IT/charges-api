using Amazon.DynamoDBv2.DataModel;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargeApi.V1.Gateways
{
    public class DynamoDbGateway : IChargeApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public async Task AddAsync(Charge charge)
        {
            await _dynamoDbContext.SaveAsync(charge.ToDatabase()).ConfigureAwait(false);
        }

        public async Task<List<Charge>> GetAllChargesAsync(string type, Guid targetId)
        {
            //ScanCondition scanCondition_id = new ScanCondition("Id", Amazon.DynamoDBv2.DocumentModel.ScanOperator.GreaterThan, new Guid("00000000-0000-0000-0000-000000000000"));

            List<ScanCondition> scanConditions = new List<ScanCondition>();
            //ToDO: Remove validation from Gateway
            if (type != null)
            {
                scanConditions.Add(new ScanCondition("TargetType", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, Enum.Parse(typeof(TargetType), type)));
            }

            if (targetId != Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                scanConditions.Add(new ScanCondition("TargetId", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, targetId));
            }

            List<ChargeDbEntity> data = await _dynamoDbContext.ScanAsync<ChargeDbEntity>(scanConditions).GetRemainingAsync().ConfigureAwait(false);

            return data.Select(p => p.ToDomain()).ToList();
        }

        public async Task<Charge> GetChargeByIdAsync(Guid id)
        {
            var result = await _dynamoDbContext.LoadAsync<ChargeDbEntity>(id).ConfigureAwait(false);
            return result?.ToDomain();
        }

        public async Task RemoveAsync(Charge charge)
        {
            await _dynamoDbContext.DeleteAsync(charge.ToDatabase()).ConfigureAwait(false);
        }

        public async Task UpdateAsync(Charge charge)
        {
            await _dynamoDbContext.SaveAsync(charge.ToDatabase()).ConfigureAwait(false);
        }
    }
}
