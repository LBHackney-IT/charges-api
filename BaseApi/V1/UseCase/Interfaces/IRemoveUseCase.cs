using BaseApi.V1.Boundary.Response;
using BaseApi.V1.Domain;
using System;
using System.Threading.Tasks;

namespace BaseApi.V1.UseCase.Interfaces
{
    public interface IRemoveUseCase
    {
        public void Execute(Guid id);
        public Task ExecuteAsync(Guid id);
    }
}
