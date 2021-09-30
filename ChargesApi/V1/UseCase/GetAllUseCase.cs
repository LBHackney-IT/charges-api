using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class GetAllUseCase : IGetAllUseCase
    {
        private readonly IChargesApiGateway _gateway;

        public GetAllUseCase(IChargesApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<List<ChargeResponse>> ExecuteAsync( string type)
        {
            var charges = (await _gateway.GetAllChargesAsync(type).ConfigureAwait(false)).ToResponse();

            return charges;
        }
    }
}
