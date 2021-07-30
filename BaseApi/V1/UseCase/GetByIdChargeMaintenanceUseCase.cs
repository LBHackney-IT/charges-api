using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase
{
    public class GetByIdChargeMaintenanceUseCase : IGetByIdChargeMaintenanceUseCase
    {
        private readonly IChargeMaintenanceApiGateway _gateway;

        public GetByIdChargeMaintenanceUseCase(IChargeMaintenanceApiGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<ChargeMaintenanceResponse> ExecuteAsync(Guid id)
        {
            var charge = await _gateway.GetChargeMaintenanceByIdAsync(id).ConfigureAwait(false);
            return charge?.ToResponse();
        }
    }
}
