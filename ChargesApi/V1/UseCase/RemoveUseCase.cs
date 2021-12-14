using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class RemoveUseCase : IRemoveUseCase
    {
        private readonly IChargesApiGateway _gateway;

        public RemoveUseCase(IChargesApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<bool> ExecuteAsync(Guid id, Guid targetId)
        {
            Charge charge = await _gateway.GetChargeByIdAsync(id, targetId).ConfigureAwait(false);

            if (charge == null)
            {
                return false;
            }

            await _gateway.RemoveAsync(charge).ConfigureAwait(false);
            return true;
        }
    }
}
