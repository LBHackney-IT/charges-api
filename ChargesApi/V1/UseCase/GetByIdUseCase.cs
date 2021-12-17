using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class GetByIdUseCase : IGetByIdUseCase
    {
        private IChargesApiGateway _gateway;

        public GetByIdUseCase(IChargesApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<ChargeResponse> ExecuteAsync(Guid id, Guid targetId)
        {
            var charge = await _gateway.GetChargeByIdAsync(id, targetId).ConfigureAwait(false);
            return charge?.ToResponse();
        }
    }
}
