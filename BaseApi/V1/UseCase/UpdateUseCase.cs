using ChargeApi.V1.Boundary.Request;
using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase
{
    public class UpdateUseCase : IUpdateUseCase
    {
        private readonly IChargeApiGateway _gateway;

        public UpdateUseCase(IChargeApiGateway gateway)
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
