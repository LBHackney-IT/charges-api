using Amazon.DynamoDBv2.DataModel;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Infrastructure.Entities;
using System;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways
{
    public class ChargeMaintenanceGateway : IChargeMaintenanceApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public ChargeMaintenanceGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public void Add(ChargeMaintenance chargeMaintenance)
        {
            _dynamoDbContext.SaveAsync(chargeMaintenance.ToDatabase());
        }

        public async Task AddAsync(ChargeMaintenance chargeMaintenance)
        {
            await _dynamoDbContext.SaveAsync(chargeMaintenance.ToDatabase()).ConfigureAwait(false);
        }

        public async Task<ChargeMaintenance> GetChargeMaintenanceByIdAsync(Guid id)
        {
            var chargeMaintenance = await _dynamoDbContext.LoadAsync<ChargesMaintenanceDbEntity>(id).ConfigureAwait(false);
            return chargeMaintenance?.ToDomain();
        }
    }
}
