using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class DeleteBatchChargesUseCase : IDeleteBatchChargesUseCase
    {
        private const int BatchCapacity = 25;
        private readonly IChargesApiGateway _chargesApiGateway;

        public DeleteBatchChargesUseCase(IChargesApiGateway chargesApiGateway)
        {
            _chargesApiGateway = chargesApiGateway;
        }

        public async Task ExecuteAsync(short chargeYear, ChargeGroup chargeGroup, ChargeSubGroup? chargeSubGroup)
        {
            var chargeIdsToDelete = await _chargesApiGateway.ScanByYearGroupSubGroup(chargeYear, chargeGroup, chargeSubGroup)
                .ConfigureAwait(false);

            if (chargeIdsToDelete == null || !chargeIdsToDelete.Any())
            {
                return;
            }

            await _chargesApiGateway.DeleteBatchAsync(chargeIdsToDelete, BatchCapacity).ConfigureAwait(false);
        }
    }
}
