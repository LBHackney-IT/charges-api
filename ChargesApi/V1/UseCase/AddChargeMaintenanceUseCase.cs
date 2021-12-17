using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
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
