using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.Infrastructure.JWT;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class AddUseCase : IAddUseCase
    {
        private readonly IChargesApiGateway _gateway;

        public AddUseCase(IChargesApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<ChargeResponse> ExecuteAsync(AddChargeRequest charge, string token)
        {
            if (charge == null)
            {
                throw new ArgumentNullException(nameof(charge));
            }

            var domainModel = charge.ToDomain();

            domainModel.Id = Guid.NewGuid();
            domainModel.CreatedBy = Helper.GetUserName(token);
            domainModel.CreatedAt = DateTime.UtcNow;
            await _gateway.AddAsync(domainModel).ConfigureAwait(false);
            return domainModel.ToResponse();
        }
    }
}
