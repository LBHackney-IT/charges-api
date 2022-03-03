using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IGetPropertyChargesUseCase
    {
        Task<List<ChargeResponse>> ExecuteAsync(PropertyChargesQueryParameters queryParameters);
    }
}
