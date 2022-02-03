using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways.Extensions;
using ChargesApi.V1.Gateways.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways.Services
{
    public class FinancialSummaryService : IFinancialSummaryService
    {
        private readonly HttpClient _client;

        public FinancialSummaryService(HttpClient client)
        {
            _client = client;
        }
        public async Task<bool> AddEstimateSummary(AddAssetSummaryRequest addAssetSummaryRequest)
        {
            var response = await _client.PostAsJsonAsyncType(new Uri("api/v1/asset-summary", UriKind.Relative), addAssetSummaryRequest)
                .ConfigureAwait(true);
            if (response)
                return true;
            else
                return false;
        }
    }
}
