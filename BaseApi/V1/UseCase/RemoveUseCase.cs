using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetAllClaimantsUseCase
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
            _gateway.Remove(charge);
        }
    }
}
