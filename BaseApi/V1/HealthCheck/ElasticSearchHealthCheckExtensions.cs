
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HousingSearchApi.Tests")]

namespace BaseApi.V1.HealthCheck
{
    public static class ElasticSearchHealthCheckExtensions
    {
        private const string Name = "Elastic search";

        internal static IServiceCollection RegisterElasticSearchHealthCheck(this IServiceCollection services)
        {
            return services.AddSingleton<IHealthCheck, ElasticSearchHealthCheck>();
        }

        internal static IHealthChecksBuilder AddElasticSearchHealthCheck(this IHealthChecksBuilder builder)
        {
            return builder.AddCheck<ElasticSearchHealthCheck>(Name);
        }


        /// <summary>
        /// Adds a health check to verify connectivity to the Elastic Search instance
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddElasticSearchHealthCheck(this IServiceCollection services)
        {
            services.RegisterElasticSearchHealthCheck();
            services.AddHealthChecks()
                    .AddElasticSearchHealthCheck();
            return services;
        }
    }
}
