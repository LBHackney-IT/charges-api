using System.Collections.Generic;
using System.Threading.Tasks;
using ChargesApi.V1.Domain;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IRemoveRangeUseCase
    {
        public Task ExecuteAsync(List<ChargeKeys> keys);
    }
}
