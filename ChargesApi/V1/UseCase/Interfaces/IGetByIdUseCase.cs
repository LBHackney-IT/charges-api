using ChargesApi.V1.Boundary.Response;
using System;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        Task<ChargeResponse> ExecuteAsync(Guid id);
    }
}
