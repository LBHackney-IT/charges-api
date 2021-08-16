using Amazon.DynamoDBv2.DataModel;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using System;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways
{
    public class ChargeMaintenanceGateway : IChargeMaintenanceApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly DynamoDbContextWrapper _wrapper;

        public ChargeMaintenanceGateway(IDynamoDBContext dynamoDbContext, DynamoDbContextWrapper wrapper)
        {
            _dynamoDbContext = dynamoDbContext;
            _wrapper = wrapper;
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
            var chargeMaintenance = await _wrapper.LoadAsync(_dynamoDbContext, id).ConfigureAwait(false);
            return chargeMaintenance?.ToDomain();
        }
    }
}
