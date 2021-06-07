using BaseApi.V1.Boundary.Response;
using System;
using System.Threading.Tasks;

namespace BaseApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        ChargeResponseObject Execute(Guid id);
        Task<ChargeResponseObject> ExecuteAsync(Guid id);
    }
}
