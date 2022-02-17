using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IDeleteBatchChargesUseCase
    {
        Task ExecuteAsync(short chargeYear, string chargeGroup, string chargeSubGroup);
    }
}
