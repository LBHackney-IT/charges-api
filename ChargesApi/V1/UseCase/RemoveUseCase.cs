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

        public async Task ExecuteAsync(Guid id)
        {
            Charge charge = await _gateway.GetChargeByIdAsync(id).ConfigureAwait(false);

            if (charge == null)
            {
                throw new Exception($"Cannot find charge with provided id: {id}");
            }

            await _gateway.RemoveAsync(charge).ConfigureAwait(false);
        }
    }
}
