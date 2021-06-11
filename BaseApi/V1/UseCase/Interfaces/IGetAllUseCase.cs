using ChargeApi.V1.Boundary.Response;
using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase.Interfaces
{
    public interface IGetAllUseCase
    {
        public ChargeResponseObjectList Execute(string type, Guid targetid);
        public Task<ChargeResponseObjectList> ExecuteAsync(string type,Guid targetid);
    }
}

