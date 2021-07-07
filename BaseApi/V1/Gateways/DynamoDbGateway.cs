using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
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
        private readonly DynamoDbContextWrapper _wrapper;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext,
            DynamoDbContextWrapper wrapper)
        {
            _dynamoDbContext = dynamoDbContext;
            _wrapper = wrapper;
        }

        public async Task AddAsync(Charge charge)
        {
            await _dynamoDbContext.SaveAsync(charge.ToDatabase()).ConfigureAwait(false);
        }

        public async Task<List<Charge>> GetAllChargesAsync(string type, Guid targetId)
        {
            //ScanCondition scanCondition_id = new ScanCondition("Id", Amazon.DynamoDBv2.DocumentModel.ScanOperator.GreaterThan, new Guid("00000000-0000-0000-0000-000000000000"));

            List<ScanCondition> scanConditions = new List<ScanCondition>();

            if (type != null)
            {
                scanConditions.Add(new ScanCondition("TargetType", ScanOperator.Equal, Enum.Parse(typeof(TargetType), type)));
            }

            if (targetId != Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                scanConditions.Add(new ScanCondition("TargetId", ScanOperator.Equal, targetId));
            }

            List<ChargeDbEntity> data = await _wrapper.ScanAsync(_dynamoDbContext, scanConditions).ConfigureAwait(false);

            return data.Select(p => p.ToDomain()).ToList();
        }

        public async Task<Charge> GetChargeByIdAsync(Guid id)
        {
            List<ScanCondition> scanConditions = new List<ScanCondition>
            {
                new ScanCondition("Id", ScanOperator.Equal, id)
            };

            var result = await _wrapper.ScanAsync(_dynamoDbContext, scanConditions).ConfigureAwait(false);

            return result.FirstOrDefault()?.ToDomain();
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
