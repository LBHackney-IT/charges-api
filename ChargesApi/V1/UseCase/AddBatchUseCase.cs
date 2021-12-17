using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;

namespace ChargesApi.V1.UseCase
{
    public class AddBatchUseCase : IAddBatchUseCase
    {
        private readonly IChargesApiGateway _gateway;

        public AddBatchUseCase(IChargesApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<int> ExecuteAsync(IEnumerable<AddChargeRequest> charges)
        {
            var chargesList = charges.ToList().ToDomainList();
            chargesList.ForEach(item =>
            {
                item.Id = Guid.NewGuid();
            });

            var response = await _gateway.AddBatchAsync(chargesList).ConfigureAwait(false);
            return chargesList.Count;
        }
    }
}
