using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nest;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChargeApi.V1.HealthCheck
{
    public class ElasticSearchHealthCheck : IHealthCheck
    {
        private readonly IElasticClient _esClient;

        public ElasticSearchHealthCheck(IElasticClient esClient)
        {
            _esClient = esClient;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var esNodes = string.Join(';', _esClient.ConnectionSettings.ConnectionPool.Nodes.Select(x => x.Uri));
            try
            {
                var pingResult = await _esClient.PingAsync(ct: cancellationToken).ConfigureAwait(false);
                var isSuccess = pingResult.ApiCall.HttpStatusCode == 200;

                return isSuccess
                    ? HealthCheckResult.Healthy($"Can successfully access the Elastic Search instance on: {esNodes}")
                    : HealthCheckResult.Unhealthy($"Cannot access the Elastic Search instance on: {esNodes}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Cannot access the Elastic Search instance on: {esNodes}", exception: ex);
            }
        }
    }
}
