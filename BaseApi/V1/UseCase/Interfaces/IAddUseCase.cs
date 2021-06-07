using BaseApi.V1.Boundary.Response;
using BaseApi.V1.Domain;
using System.Threading.Tasks;

namespace BaseApi.V1.UseCase.Interfaces
{
    public interface IAddUseCase
    {
        public ChargeResponseObject Execute(Charge charge);
        public Task<ChargeResponseObject> ExecuteAsync(Charge charge);
    }
}
