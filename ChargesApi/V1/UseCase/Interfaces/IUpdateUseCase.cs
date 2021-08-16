using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IUpdateUseCase
    {
        public ChargeResponse Execute(UpdateChargeRequest charge);
        public Task<ChargeResponse> ExecuteAsync(UpdateChargeRequest charge);
    }
}
