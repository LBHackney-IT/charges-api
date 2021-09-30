using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IAddActualsUseCase
    {
        Task<int> AddActuals(IFormFile file);
    }
}
