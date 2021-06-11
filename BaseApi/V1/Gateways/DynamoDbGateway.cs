using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public void Add(Charge charge)
        {
            _dynamoDbContext.SaveAsync<ChargeDbEntity>(charge.ToDatabase());
        }

        public async Task AddAsync(Charge charge)
        {
            await _dynamoDbContext.SaveAsync<ChargeDbEntity>(charge.ToDatabase()).ConfigureAwait(false);
        }

        public void AddRange(List<Charge> charges)
        {
            charges.ForEach(e =>
            {
                _dynamoDbContext.SaveAsync<ChargeDbEntity>(e.ToDatabase());
            });
        }

        public async Task AddRangeAsync(List<Charge> charges)
        {
            foreach (Charge charge in charges)
            {
                await _dynamoDbContext.SaveAsync<ChargeDbEntity>(charge.ToDatabase()).ConfigureAwait(false);
            }
        }

        public void CalculateCharges(Guid targetId, string targetType)
        {
            throw new NotImplementedException();
        }

        public Task CalculateChargesAsync(Guid targetId, string targetType)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Charge>> GetAllChargesAsync(string type, Guid targetid)
        {
            //ScanCondition scanCondition_id = new ScanCondition("Id", Amazon.DynamoDBv2.DocumentModel.ScanOperator.GreaterThan, new Guid("00000000-0000-0000-0000-000000000000"));
            ScanCondition scanCondition_type = new ScanCondition("TargetType", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, Enum.Parse(typeof(TargetType), type));
            ScanCondition scanCondition_targetid = new ScanCondition("TargetId", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, targetid);

            List<ScanCondition> scanConditions = new List<ScanCondition>();

            if (type != null)
                scanConditions.Add(scanCondition_type);
            if(targetid != Guid.Parse("00000000-0000-0000-0000-000000000000"))
                scanConditions.Add(scanCondition_targetid);

            List<ChargeDbEntity> data = await _dynamoDbContext.ScanAsync<ChargeDbEntity>(scanConditions).GetRemainingAsync().ConfigureAwait(false);
            return data.Select(p=>p.ToDomain()).ToList();
        }

        public Charge GetChargeById(Guid id)
        {
            var result = _dynamoDbContext.LoadAsync<ChargeDbEntity>(id).GetAwaiter().GetResult();
            return result?.ToDomain();
        }

        public async Task<Charge> GetChargeByIdAsync(Guid id)
        {
            var result = await _dynamoDbContext.LoadAsync<ChargeDbEntity>(id).ConfigureAwait(false);
            return result?.ToDomain();
        }

        public void Remove(Charge charge)
        {
            _dynamoDbContext.DeleteAsync<ChargeDbEntity>(charge.ToDatabase());
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
            _dynamoDbContext.SaveAsync<ChargeDbEntity>(charge.ToDatabase()).ConfigureAwait(false);
        }

        public async Task UpdateAsync(Charge charge)
        {
            await _dynamoDbContext.SaveAsync<ChargeDbEntity>(charge.ToDatabase()).ConfigureAwait(false);
        }
    }
}
