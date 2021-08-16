using ChargeApi.V1.Domain;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase
{
    public class RemoveUseCase : IRemoveUseCase
    {
        private readonly IChargeApiGateway _gateway;

        public RemoveUseCase(IChargeApiGateway gateway)
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
