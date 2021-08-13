using ChargeApi.V1.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase.Interfaces
{
    public interface IGetAllUseCase
    {
        public Task<List<ChargeResponse>> ExecuteAsync(string type,Guid targetid);
    }
}

