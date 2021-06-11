using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Domain;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase.Interfaces
{
    public interface IUpdateUseCase
    {
        public ChargeResponseObject Execute(Charge charge);
        public Task<ChargeResponseObject> ExecuteAsync(Charge charge);
    }
}
