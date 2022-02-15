using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<ChargeResponse>> ExecuteAsync(Guid targetId)
        {
            var charges = await _gateway.GetAllChargesAsync(targetId).ConfigureAwait(false);

            // Get the latest version number
            var latestVersionId = charges.Select(c => c.VersionId).Distinct().ToList().Max();
            if (latestVersionId > 0)
            {
                charges = charges.Where(c => c.VersionId == latestVersionId).ToList();
            }

            return charges.ToResponse();
        }
    }
}
