using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase.Interfaces
{
    public interface IRemoveUseCase
    {
        public Task ExecuteAsync(Guid id);
    }
}
