using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ChargesApi.V1.Infrastructure
{
    public static class ModelStateExtension
    {
        public static string GetErrorMessages(this ModelStateDictionary modelState)
        {
            return
                string.Join(",", modelState.SelectMany(e => e.Value.Errors.Select(s => s.ErrorMessage)));
        }

        public static string GetErrorMessages(this ValidationResult validationResult)
        {
            return string.Join(",", validationResult.Errors.Select(_ => _.ErrorMessage));
        }
    }
}
