using BaseApi.V1.Boundary.Response;
using System;
using System.Threading.Tasks;

namespace BaseApi.V1.UseCase.Interfaces
{
    public interface IGetAllUseCase
    {
        public ChargeResponseObjectList Execute(string type, Guid targetid);
        public Task<ChargeResponseObjectList> ExecuteAsync(string type,Guid targetid);
    }
}

