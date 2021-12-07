using System.Collections.Generic;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IAddBatchUseCase
    {
        public Task<int> ExecuteAsync(IEnumerable<AddChargeRequest> charges);
    }
}
