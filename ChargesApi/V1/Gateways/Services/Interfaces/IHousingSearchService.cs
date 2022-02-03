using ChargesApi.V1.Domain;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways.Services.Interfaces
{
    public interface IHousingSearchService
    {
        Task<AssetListResponse> GetAssets(string type, int pageSize, int pageNumber, string lastHitId = null);
    }
}
