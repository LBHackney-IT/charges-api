using BaseApi.V1.Boundary.Response;
using BaseApi.V1.Domain;
using BaseApi.V1.Factories;
using BaseApi.V1.Gateways;
using BaseApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace BaseApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetAllClaimantsUseCase
    public class UpdateUseCase : IUpdateUseCase
    {
        private readonly IChargeApiGateway _gateway;
        public UpdateUseCase(IChargeApiGateway gateway)
        {
            _gateway = gateway;
        }

        public ChargeResponseObject Execute(Charge charge)
        {
            _gateway.Update(charge);
            return charge.ToResponse();
        }

        public async Task<ChargeResponseObject> ExecuteAsync(Charge charge)
        {
            await _gateway.UpdateAsync(charge).ConfigureAwait(false);
            return charge.ToResponse();
        }
    }
}
