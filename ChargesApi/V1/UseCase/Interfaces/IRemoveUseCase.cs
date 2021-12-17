using System;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IRemoveUseCase
    {
        public Task<bool> ExecuteAsync(Guid id, Guid targetId);
    }
}
