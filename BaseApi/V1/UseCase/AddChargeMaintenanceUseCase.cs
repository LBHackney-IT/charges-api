using ChargeApi.V1.Boundary.Request;
using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase
{
    public class AddChargeMaintenanceUseCase : IAddChargeMaintenanceUseCase
    {
        private readonly IChargeMaintenanceApiGateway _gateway;

        public AddChargeMaintenanceUseCase(IChargeMaintenanceApiGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<ChargeMaintenanceResponse> ExecuteAsync(AddChargeMaintenanceRequest chargeMaintenance)
        {
            if (chargeMaintenance == null)
            {
                throw new ArgumentNullException(nameof(chargeMaintenance));
            }

            var domainModel = chargeMaintenance.ToDomain();

            domainModel.Id = Guid.NewGuid();

            await _gateway.AddAsync(domainModel).ConfigureAwait(false);
            return domainModel?.ToResponse();
        }
    }
}
