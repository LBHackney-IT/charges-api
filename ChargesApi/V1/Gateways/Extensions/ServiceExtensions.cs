using System;
using Amazon.S3;
using ChargesApi.Configuration;
using ChargesApi.V1.Gateways.Services;
using ChargesApi.V1.Gateways.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChargesApi.V1.Gateways.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddAmazonS3(this IServiceCollection services, IHostEnvironment environment,
            IConfiguration configuration)
        {
            services.Configure<S3ConfigurationOptions>(configuration.GetSection(S3ConfigurationOptions.SectionName));
            services.AddScoped<IAwsS3FileService, AwsS3FileService>();

            if (environment.IsDevelopment())
            {
                services.AddScoped<IAmazonS3, AmazonS3Emulator>();
                services.AddHttpClient<IAmazonS3, AmazonS3Emulator>(client =>
                {
                    client.BaseAddress = new Uri(configuration[$"{S3ConfigurationOptions.SectionName}:DevelopmentUrl"]);
                    client.Timeout = TimeSpan.FromMinutes(30); // For debugging purposes
                });
            }
            else
            {
                services.AddAWSService<IAmazonS3>();
            }
        }
    }
}
