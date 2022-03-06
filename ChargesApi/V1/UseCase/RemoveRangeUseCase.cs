using System.Collections.Generic;
using System.Threading.Tasks;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;

namespace ChargesApi.V1.UseCase
{
    public class RemoveRangeUseCase : IRemoveRangeUseCase
    {
        private readonly IChargesApiGateway _gateway;

        public RemoveRangeUseCase(IChargesApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task ExecuteAsync(List<ChargeKeys> keys)
        {
            await _gateway.RemoveRangeAsync(keys).ConfigureAwait(false);
        }
    }
}
