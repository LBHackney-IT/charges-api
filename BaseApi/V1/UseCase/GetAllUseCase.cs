using BaseApi.V1.Boundary.Response;
using BaseApi.V1.Domain;
using BaseApi.V1.Factories;
using BaseApi.V1.Gateways;
using BaseApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetAllClaimantsUseCase
    public class GetAllUseCase : IGetAllUseCase
    {
        private readonly IChargeApiGateway _gateway;
        public GetAllUseCase(IChargeApiGateway gateway)
        {
            _gateway = gateway;
        }

        public ChargeResponseObjectList Execute(string type, Guid targetid)
        {
            return new ChargeResponseObjectList { ChargeResponseObjects = _gateway.GetAllCharges(type,targetid).ToResponse() };
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
