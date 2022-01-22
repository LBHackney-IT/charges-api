using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class GetAllChargesListUseCase : IGetAllChargesListUseCase
    {
        private readonly IChargesListApiGateway _gateway;

        public GetAllChargesListUseCase(IChargesListApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<List<ChargesListResponse>> ExecuteAsync(string chargeCode)
        {
            var result = await _gateway.GetAllChargesListAsync(chargeCode.ToUpper()).ConfigureAwait(false);

            return result?.ToResponse();
        }
    }
}
