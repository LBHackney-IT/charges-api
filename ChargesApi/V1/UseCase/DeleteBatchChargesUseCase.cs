using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Infrastructure;

namespace ChargesApi.V1.UseCase
{
    public class DeleteBatchChargesUseCase : IDeleteBatchChargesUseCase
    {
        private readonly IChargesApiGateway _chargesApiGateway;

        public DeleteBatchChargesUseCase(IChargesApiGateway chargesApiGateway)
        {
            _chargesApiGateway = chargesApiGateway;
        }

        public async Task ExecuteAsync(short chargeYear, ChargeGroup chargeGroup, ChargeSubGroup? chargeSubGroup)
        {
            if (chargeSubGroup != null)
            {
                var request = new PropertyChargesQueryParameters
                {
                    ChargeGroup = chargeGroup,
                    ChargeSubGroup = chargeSubGroup.Value,
                    ChargeYear = chargeYear
                };
                var chargeIdsToDelete = await _chargesApiGateway.GetChargesAsync(request)
                    .ConfigureAwait(false);

                if (chargeIdsToDelete == null || !chargeIdsToDelete.Any())
                {
                    return;
                }

                var chargeKeys = chargeIdsToDelete.Select(x => x.GetChargeKeys()).AsEnumerable();
                await _chargesApiGateway.DeleteBatchAsync(chargeKeys, Constants.PerBatchProcessingCount).ConfigureAwait(false);
            }
        }
    }
}
