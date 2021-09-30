using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Infrastructure;
using ChargesApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace ChargesApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/estimates")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class EstimatesController : BaseController
    {
        private readonly IAddEstimatesUseCase _addEstimatesUseCase;

        public EstimatesController(IAddEstimatesUseCase addEstimatesUseCase)
        {
            _addEstimatesUseCase = addEstimatesUseCase;
        }

        /// <summary>
        /// Create new List of Estimates records by batch processing
        /// </summary>
        /// <param name="addEstimatesRequest">Estimates File model for create</param>
        /// <response code="201">Success. Estimates reccords was created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm]AddEstimatesRequest addEstimatesRequest)
        {
            if (addEstimatesRequest == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Add Estimate request cannot be null!"));
            }
            if (ModelState.IsValid)
            {
                int processingCount = 0;
                if (addEstimatesRequest != null)
                {
                    processingCount = await _addEstimatesUseCase.AddEstimates(addEstimatesRequest.EstimatesFile).ConfigureAwait(false);
                }
                return Ok($"{processingCount} estimates records processed successfully");
            }
            else
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, ModelState.GetErrorMessages()));
            }
        }

        
    }
}
