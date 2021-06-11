using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetAllClaimantsUseCase
    public class GetAllUseCase : IGetAllUseCase
    {
        private readonly IChargeApiGateway _gateway;
        public GetAllUseCase(IChargeApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<ChargeResponseObjectList> ExecuteAsync(string type,Guid targetid)
        {
            ChargeResponseObjectList chargeResponseObjectList = new ChargeResponseObjectList();
            List<Charge> data = await _gateway.GetAllChargesAsync(type,targetid).ConfigureAwait(false);

            chargeResponseObjectList.ChargeResponseObjects = data.Select(p => p.ToResponse()).ToList();

            return chargeResponseObjectList;
        }
    }
}
