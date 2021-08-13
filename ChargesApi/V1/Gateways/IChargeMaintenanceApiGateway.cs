using ChargeApi.V1.Domain;
using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.Gateways
{
    public interface IChargeMaintenanceApiGateway
    {
        public Task<ChargeMaintenance> GetChargeMaintenanceByIdAsync(Guid id);

        public void Add(ChargeMaintenance chargeMaintenance);
        public Task AddAsync(ChargeMaintenance chargeMaintenance);
    }
}
