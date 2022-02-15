using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;

namespace ChargesApi.V1.UseCase
{
    public class GetPropertyChargesUseCase : IGetPropertyChargesUseCase
    {
        private readonly IChargesApiGateway _gateway;

        public GetPropertyChargesUseCase(IChargesApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<List<ChargeResponse>> ExecuteAsync(PropertyChargesQueryParameters queryParameters)
        {
            var result = await _gateway.GetChargesAsync(queryParameters).ConfigureAwait(false);

            return result.ToResponse();
        }
    }
}
