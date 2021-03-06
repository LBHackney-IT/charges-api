using Amazon.SimpleNotificationService;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using ChargesApi.V1;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Controllers;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.Gateways.Common;
using ChargesApi.V1.Gateways.Extensions;
using ChargesApi.V1.Gateways.Services;
using ChargesApi.V1.Gateways.Services.Interfaces;
using ChargesApi.V1.Infrastructure;
using ChargesApi.V1.Infrastructure.Validators;
using ChargesApi.V1.UseCase;
using ChargesApi.V1.UseCase.Interfaces;
using ChargesApi.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hackney.Core.Authorization;
using Hackney.Core.JWT;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

namespace ChargesApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            CurrentEnvironment = environment;

            AWSSDKHandler.RegisterXRayForAllServices();
        }

        public IConfiguration Configuration { get; }
        private IHostEnvironment CurrentEnvironment { get; }
        private static List<ApiVersionDescription> _apiVersions { get; set; }

        //TODO update the below to the name of your API
        private const string ApiName = "charges";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddNewtonsoftJson(opts => opts.SerializerSettings.Converters.Add(new StringEnumConverter()))
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true; // assume that the caller wants the default version if they don't specify
                o.ApiVersionReader = new UrlSegmentApiVersionReader(); // read the version number from the url segment header)
            });
            services.AddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Your Hackney Token. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });

                //Looks at the APIVersionAttribute [ApiVersion("x")] on controllers and decides whether or not
                //to include it in that version of the swagger document
                //Controllers must have this [ApiVersion("x")] to be included in swagger documentation!!
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    apiDesc.TryGetMethodInfo(out var methodInfo);

                    var versions = methodInfo?
                        .DeclaringType?.GetCustomAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions).ToList();

                    return versions?.Any(v => $"{v.GetFormattedApiVersion()}" == docName) ?? false;
                });

                //Get every ApiVersion attribute specified and create swagger docs for them
                foreach (var apiVersion in _apiVersions)
                {
                    var version = $"v{apiVersion.ApiVersion.ToString()}";
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Title = $"{ApiName}-api {version}",
                        Version = version,
                        Description = $"{ApiName} version {version}. Please check older versions for depreciated endpoints."
                    });
                }

                c.CustomSchemaIds(x => x.FullName);
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            ConfigureLogging(services, Configuration);
            services.ConfigureLambdaLogging(Configuration);
            services.AddTokenFactory();
            //ConfigureDbContext(services);
            //TODO: For DynamoDb, remove the line above and uncomment the line below.
            services.ConfigureDynamoDB();
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSimpleNotificationService>();
            services.AddLogCallAspect();
            services.AddAmazonS3(CurrentEnvironment, Configuration);
            RegisterGateways(services);
            RegisterUseCases(services);

            services.AddFluentValidation(config =>
            {
                config.ValidatorOptions.LanguageManager.Culture = new CultureInfo("en");
                config.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            });
            RegisterValidators(services);

            services.AddCors(opt => opt.AddPolicy("corsPolicy", builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()));
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddHttpContextAccessor();
        }

        private static void ConfigureDbContext(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            services.AddDbContext<ChargeContext>(
                opt => opt.UseNpgsql(connectionString).AddXRayInterceptor(true));
        }

        private static void ConfigureLogging(IServiceCollection services, IConfiguration configuration)
        {
            // We rebuild the logging stack so as to ensure the console logger is not used in production.
            // See here: https://weblog.west-wind.com/posts/2018/Dec/31/Dont-let-ASPNET-Core-Default-Console-Logging-Slow-your-App-down
            services.AddLogging(config =>
            {
                // clear out default configuration
                config.ClearProviders();

                config.AddConfiguration(configuration.GetSection("Logging"));
                config.AddDebug();
                config.AddEventSourceLogger();

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
                {
                    config.AddConsole();
                }
            });
        }

        private static void RegisterGateways(IServiceCollection services)
        {
            services.AddScoped<IChargesApiGateway, ChargesApiGateway>();
            services.AddScoped<IChargeMaintenanceApiGateway, ChargeMaintenanceGateway>();
            services.AddScoped<IChargesListApiGateway, ChargesListApiGateway>();
            services.AddScoped<ISnsGateway, ChargesSnsGateway>();

            services.AddTransient<LoggingDelegatingHandler>();

            var housingSearchApiUrl = Environment.GetEnvironmentVariable("HOUSING_SEARCH_API_URL");
            //var housingSearchApiKey = Environment.GetEnvironmentVariable("HOUSING_SEARCH_API_KEY");
            var housingSearchApiToken = Environment.GetEnvironmentVariable("HOUSING_SEARCH_API_TOKEN");

            var fakeUri = $"http://4590345803984584058034850348.test";
            var fakeToken = $"test-token";
            services.AddHttpClient<IHousingSearchService, HousingSearchService>(c =>
            {
                c.BaseAddress = new Uri(housingSearchApiUrl ?? fakeUri);
                //c.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", housingSearchApiKey);
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(housingSearchApiToken ?? fakeToken);
                c.Timeout = TimeSpan.FromSeconds(20);
            })
           .AddHttpMessageHandler<LoggingDelegatingHandler>();
        }

        private static void RegisterUseCases(IServiceCollection services)
        {
            services.AddScoped<IGetAllUseCase, GetAllUseCase>();
            services.AddScoped<IGetByIdUseCase, GetByIdUseCase>();
            services.AddScoped<IAddUseCase, AddUseCase>();
            services.AddScoped<IRemoveUseCase, RemoveUseCase>();
            services.AddScoped<IRemoveRangeUseCase, RemoveRangeUseCase>();
            services.AddScoped<IUpdateUseCase, UpdateUseCase>();
            services.AddScoped<IAddChargeMaintenanceUseCase, AddChargeMaintenanceUseCase>();
            services.AddScoped<IGetByIdChargeMaintenanceUseCase, GetByIdChargeMaintenanceUseCase>();
            services.AddScoped<IAddChargesListUseCase, AddChargesListUseCase>();
            services.AddScoped<IGetAllChargesListUseCase, GetAllChargesListUseCase>();
            services.AddScoped<IGetByIdChargesListUseCase, GetByIdChargesListUseCase>();
            services.AddScoped<ISnsFactory, ChargesSnsFactory>();
            services.AddScoped<IAddChargesUpdateUseCase, AddChargesUpdateUseCase>();
            services.AddScoped<IGetChargesSummaryUseCase, GetChargesSummaryUseCase>();
            services.AddScoped<IAddBatchUseCase, AddBatchUseCase>();
            services.AddScoped<IEstimateActualUploadUseCase, EstimateActualUploadUseCase>();
            services.AddScoped<IGetFileProcessingLogUseCase, GetFileProcessingLogUseCase>();
            services.AddScoped<IUpdateChargeUseCase, UpdateChargeUseCase>();
            services.AddScoped<IDeleteBatchChargesUseCase, DeleteBatchChargesUseCase>();
            services.AddScoped<IGetPropertyChargesUseCase, GetPropertyChargesUseCase>();
            services.AddScoped<IGeneratePropertyChargesFileUseCase, GeneratePropertyChargesFileUseCase>();
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            services.AddTransient<IValidator<AddChargeRequest>, AddChargeRequestValidator>();
            services.AddTransient<IValidator<UpdateChargeRequest>, UpdateChargeRequestValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("corsPolicy");
            app.UseCorrelation();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseLogCall();
            // TODO
            // If you DON'T use the renaming script, PLEASE replace with your own API name manually
            app.UseXRay("base-api");

            //Get All ApiVersions,
            var api = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            _apiVersions = api.ApiVersionDescriptions.ToList();

            //Swagger ui to view the swagger.json file
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersionDescription in _apiVersions)
                {
                    //Create a swagger endpoint for each swagger version
                    c.SwaggerEndpoint($"{apiVersionDescription.GetFormattedApiVersion()}/swagger.json",
                        $"{ApiName}-api {apiVersionDescription.GetFormattedApiVersion()}");
                }
            });
            app.UseSwagger();
            app.UseGoogleGroupAuthorization();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                // SwaggerGen won't find controllers that are routed via this technique.
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
