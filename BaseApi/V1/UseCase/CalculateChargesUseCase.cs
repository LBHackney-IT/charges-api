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
    public class CalculateChargesUseCase : ICalculateChargesUseCase
    {
        private readonly IChargeApiGateway _gateway;
        public CalculateChargesUseCase(IChargeApiGateway gateway)
        {
            _gateway = gateway;
        }

        public void Execute(Guid targetid, string targettype)
        {
            _gateway.CalculateCharges(targetid, targettype);
        }

        public async Task ExecuteAsync(Guid targetid, string targettype)
        {
            await _gateway.CalculateChargesAsync(targetid, targettype).ConfigureAwait(false);
        }
    }
}
