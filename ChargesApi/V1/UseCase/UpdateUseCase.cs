using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.Infrastructure.JWT;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class UpdateUseCase : IUpdateUseCase
    {
        private readonly IChargesApiGateway _gateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public UpdateUseCase(IChargesApiGateway gateway,
                             ISnsGateway snsGateway,
                             ISnsFactory snsFactory)
        {
            _gateway = gateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
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

        public async Task<ChargeResponse> ExecuteAsync(ChargeResponse charge, string token)
        {
            if (charge == null)
            {
                throw new ArgumentNullException(nameof(charge));
            }
            var domainModel = charge.ToDomain();
            domainModel.CreatedBy = Helper.GetUserName(token);
            domainModel.LastUpdatedAt = DateTime.UtcNow;
            await _gateway.UpdateAsync(domainModel).ConfigureAwait(false);

            var snsMessage = _snsFactory.Create(charge);
            await _snsGateway.Publish(snsMessage).ConfigureAwait(false);

            return charge;
        }
    }
}
