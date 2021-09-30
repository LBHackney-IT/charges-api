using ChargesApi.V1.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ChargesApi.V1.Boundary.Request
{
    public class AddEstimatesRequest
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".xlsx", ".xls" })]
        public IFormFile EstimatesFile { get; set; }
    }
}
