using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IAddEstimatesUseCase
    {
        Task<int> AddEstimates(IFormFile file);
    }
}
