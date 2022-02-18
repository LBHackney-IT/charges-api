using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class DeleteBatchChargesUseCase : IDeleteBatchChargesUseCase
    {
        private readonly IChargesApiGateway _chargesApiGateway;

        public DeleteBatchChargesUseCase(IChargesApiGateway chargesApiGateway)
        {
            _chargesApiGateway = chargesApiGateway;
        }

        public async Task ExecuteAsync(short chargeYear, ChargeGroup chargeGroup, ChargeSubGroup chargeSubGroup)
        {
            var charges = await _chargesApiGateway.ScanByYearGroupSubGroup(chargeYear, chargeGroup, chargeSubGroup)
                .ConfigureAwait(false);

            await _chargesApiGateway.DeleteBatchAsync(charges)
                .ConfigureAwait(false);
        }
    }
}
