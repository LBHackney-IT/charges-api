using ChargeApi.V1.Boundary.Request;
using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase
{
    public class AddUseCase : IAddUseCase
    {
        private readonly IChargeApiGateway _gateway;

        public AddUseCase(IChargeApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<ChargeResponse> ExecuteAsync(AddChargeRequest charge)
        {
            var domainModel = charge.ToDomain();

            domainModel.Id = Guid.NewGuid();

            await _gateway.AddAsync(domainModel).ConfigureAwait(false);
            return domainModel.ToResponse();
        }
    }
}
