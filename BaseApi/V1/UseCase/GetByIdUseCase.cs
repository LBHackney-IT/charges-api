using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetClaimantByIdUseCase
    public class GetByIdUseCase : IGetByIdUseCase
    {
        private IChargeApiGateway _gateway;
        public GetByIdUseCase(IChargeApiGateway gateway)
        {
            _gateway = gateway;
        }

        //TODO: rename id to the name of the identifier that will be used for this API, the type may also need to change
        public ChargeResponseObject Execute(Guid id)
        {
            return _gateway.GetChargeById(id).ToResponse();
        }

        public async Task<ChargeResponseObject> ExecuteAsync(Guid id)
        {
            var charge = await _gateway.GetChargeByIdAsync(id).ConfigureAwait(false);
            if (charge == null)
                return null;
            return charge.ToResponse();
        }
    }
}
