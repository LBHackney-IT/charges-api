using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class AddChargesUpdateUseCase : IAddChargesUpdateUseCase
    {
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public AddChargesUpdateUseCase(ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<ChargesUpdateResponse> ExecuteAsync(AddChargesUpdateRequest charges)
        {
            var publishMessage = _snsFactory.Create(charges);
            await _snsGateway.Publish(publishMessage).ConfigureAwait(false);
            return new ChargesUpdateResponse { IsSuccess = true };
        }
    }
}
