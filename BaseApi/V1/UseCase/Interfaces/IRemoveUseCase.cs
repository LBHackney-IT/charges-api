using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Domain;
using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase.Interfaces
{
    public interface IRemoveUseCase
    {
        public Task ExecuteAsync(Guid id);
    }
}
