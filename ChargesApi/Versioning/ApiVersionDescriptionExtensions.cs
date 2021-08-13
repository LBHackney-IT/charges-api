using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ChargeApi.Versioning
{
    public static class ApiVersionDescriptionExtensions
    {
        public static string GetFormattedApiVersion(this ApiVersionDescription apiVersionDescription)
        {
            return $"v{apiVersionDescription.ApiVersion.ToString()}";
        }
    }
}

