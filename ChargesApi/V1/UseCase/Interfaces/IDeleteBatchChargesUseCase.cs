using ChargesApi.V1.Domain;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IDeleteBatchChargesUseCase
    {
        Task ExecuteAsync(short chargeYear, ChargeGroup chargeGroup, ChargeSubGroup? chargeSubGroup);
    }
}
