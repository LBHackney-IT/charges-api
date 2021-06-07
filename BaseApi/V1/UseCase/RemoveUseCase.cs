using BaseApi.V1.Boundary.Response;
using BaseApi.V1.Domain;
using BaseApi.V1.Factories;
using BaseApi.V1.Gateways;
using BaseApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace BaseApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetAllClaimantsUseCase
    public class RemoveUseCase : IRemoveUseCase
    {
        private readonly IChargeApiGateway _gateway;
        public RemoveUseCase(IChargeApiGateway gateway)
        {
            _gateway = gateway;
        }

        public void Execute(Guid id)
        {
            Charge charge = _gateway.GetChargeById(id);
            _gateway.Remove(charge);
        }

        public async Task ExecuteAsync(Guid id)
        {
            Charge charge = await _gateway.GetChargeByIdAsync(id).ConfigureAwait(false);
            _gateway.Remove(charge);
        }
    }
}
