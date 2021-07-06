using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase
{
    public class GetAllUseCase : IGetAllUseCase
    {
        private readonly IChargeApiGateway _gateway;

        public GetAllUseCase(IChargeApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<List<ChargeResponse>> ExecuteAsync(string type, Guid targetId)
        {
            // ToDO: Validate type
            var charges = (await _gateway.GetAllChargesAsync(type, targetId).ConfigureAwait(false)).ToResponse();

            return charges;
        }
    }
}
