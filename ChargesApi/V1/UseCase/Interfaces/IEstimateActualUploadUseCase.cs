using ChargesApi.V1.Domain;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IEstimateActualUploadUseCase
    {
        Task<bool> ExecuteAsync(IFormFile file, ChargeGroup chargeGroup, string token);
    }
}
