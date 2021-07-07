using ChargeApi.V1.Boundary.Request;
using ChargeApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase.Interfaces
{
    public interface IAddUseCase
    {
        public ChargeResponse Execute(AddChargeRequest charge);
        public Task<ChargeResponse> ExecuteAsync(AddChargeRequest charge);
    }
}
