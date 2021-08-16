using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways
{
    public class DynamoDbGateway : IChargesApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly DynamoDbContextWrapper _wrapper;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext,
            DynamoDbContextWrapper wrapper)
        {
            _dynamoDbContext = dynamoDbContext;
            _wrapper = wrapper;
        }

        public void Add(Charge charge)
        {
            _dynamoDbContext.SaveAsync(charge.ToDatabase());
        }

        public async Task AddAsync(Charge charge)
        {
            await _dynamoDbContext.SaveAsync(charge.ToDatabase()).ConfigureAwait(false);
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

        public async Task<List<Charge>> GetAllChargesAsync(string type, Guid targetId)
        {
            List<ScanCondition> scanConditions = new List<ScanCondition>
            {
                new ScanCondition("TargetType", ScanOperator.Equal, Enum.Parse(typeof(TargetType), type)),
            };

            if (targetId != Guid.Empty)
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
    }
}
