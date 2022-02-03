using ChargesApi.V1.Domain;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways.Services.Interfaces
{
    public interface IFinancialSummaryService
    {
        Task<bool> AddEstimateSummary(AddAssetSummaryRequest addAssetSummaryRequest);
    }
}
