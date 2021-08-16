using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class UpdateUseCase : IUpdateUseCase
    {
        private readonly IChargesApiGateway _gateway;

        public UpdateUseCase(IChargesApiGateway gateway)
        {
            _gateway = gateway;
        }

        public ChargeResponse Execute(UpdateChargeRequest charge)
        {
            if (charge == null)
            {
                throw new ArgumentNullException(nameof(charge));
            }

            var domain = charge.ToDomain();

            _gateway.Update(domain);
            return domain.ToResponse();
        }

        public async Task<ChargeResponse> ExecuteAsync(UpdateChargeRequest charge)
        {
            if (charge == null)
            {
                throw new ArgumentNullException(nameof(charge));
            }

            var domain = charge.ToDomain();

            await _gateway.UpdateAsync(domain).ConfigureAwait(false);

            return domain.ToResponse();
        }
    }
}
