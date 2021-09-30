using ChargesApi.V1.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways
{
    public interface IEstimatesApiGateway
    {
        Task<bool> SaveEstimateBatch(List<Estimate> estimates);
    }
}
