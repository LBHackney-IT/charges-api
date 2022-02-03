using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways.Extensions;
using ChargesApi.V1.Gateways.Services.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways.Services
{
    public class HousingSearchService : IHousingSearchService
    {
        private readonly HttpClient _client;

        public HousingSearchService(HttpClient client)
        {
            _client = client;
        }

        public async Task<AssetListResponse> GetAssets(string assetType, int pageSize, int pageNumber, string lastHitId = null)
        {
            var uri = string.IsNullOrEmpty(lastHitId)
                ? new Uri($"api/v1/search/assets/all?sortBy=assetId&isDesc=false&assetTypes={assetType}&searchText=**&pageSize={pageSize}&page={pageNumber}", UriKind.Relative)
                : new Uri($"api/v1/search/assets/all?sortBy=assetId&isDesc=false&assetTypes={assetType}&searchText=**&pageSize={pageSize}&page={pageNumber}&lastHitId={lastHitId}", UriKind.Relative);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            var result = await response.ReadContentAs<AssetListResponse>().ConfigureAwait(true);
            return result;
        }
    }
}
