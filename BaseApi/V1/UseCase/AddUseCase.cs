using BaseApi.V1.Boundary.Response;
using BaseApi.V1.Domain;
using BaseApi.V1.Factories;
using BaseApi.V1.Gateways;
using BaseApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace BaseApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetAllClaimantsUseCase
    public class AddUseCase : IAddUseCase
    {
        private readonly IChargeApiGateway _gateway;
        public AddUseCase(IChargeApiGateway gateway)
        {
            _gateway = gateway;
        }

        public ChargeResponseObject Execute(Charge charge)
        {
            _gateway.Add(charge);
            return charge.ToResponse();
        }

        public async Task<ChargeResponseObject> ExecuteAsync(Charge charge)
        {
            await _gateway.AddAsync(charge).ConfigureAwait(false);
            return charge.ToResponse();
        }
    }
}
