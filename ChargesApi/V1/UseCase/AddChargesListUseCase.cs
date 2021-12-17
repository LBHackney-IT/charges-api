using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class AddChargesListUseCase : IAddChargesListUseCase
    {
        private readonly IChargesListApiGateway _gateway;

        public AddChargesListUseCase(IChargesListApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<ChargesListResponse> ExecuteAsync(AddChargesListRequest chargesList)
        {
            if (chargesList == null)
            {
                throw new ArgumentNullException(nameof(chargesList));
            }

            var domainModel = chargesList.ToDomain();

            domainModel.Id = Guid.NewGuid();

            await _gateway.AddAsync(domainModel).ConfigureAwait(false);
            return domainModel.ToResponse();
        }
    }
}
