using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class GetByIdChargesListUseCase : IGetByIdChargesListUseCase
    {
        private readonly IChargesListApiGateway _gateway;

        public GetByIdChargesListUseCase(IChargesListApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<ChargesListResponse> ExecuteAsync(Guid id)
        {
            var chargesList = await _gateway.GetChargesListByIdAsync(id).ConfigureAwait(false);
            return chargesList?.ToResponse();
        }
    }
}
