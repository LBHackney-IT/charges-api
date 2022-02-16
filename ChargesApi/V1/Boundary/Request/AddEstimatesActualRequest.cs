using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ChargesApi.V1.Boundary.Request
{
    public class AddEstimatesActualRequest
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".xlsx", ".xls" })]
        public IFormFile EstimatesActualFile { get; set; }

        public ChargeGroup ChargeGroup { get; set; }
    }
}
