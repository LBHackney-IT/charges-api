using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Infrastructure;
using ChargesApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ChargesApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/estimates")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class EstimatesActualUploadController : BaseController
    {
        private readonly IEstimateActualUploadUseCase _addEstimatesUseCase;

        public EstimatesActualUploadController(IEstimateActualUploadUseCase addEstimateChargesUseCase)
        {
            _addEstimatesUseCase = addEstimateChargesUseCase;
        }

        /// <summary>
        /// Create new List of Estimates records by batch processing
        /// </summary>
        /// <param name="token">User Token</param>
        /// <param name="addEstimatesActualRequest">Estimates/Actual File model for create</param>
        /// <response code="201">Success. Estimates reccords was created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> Post([FromHeader(Name = "Authorization")] string token, [FromForm] AddEstimatesActualRequest addEstimatesActualRequest)
        {
            if (addEstimatesActualRequest == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Add Estimate/Actual request cannot be null!"));
            }
            if (ModelState.IsValid)
            {
                var processingResult = await _addEstimatesUseCase.ExecuteAsync(addEstimatesActualRequest.EstimatesActualFile,
                    addEstimatesActualRequest.ChargeGroup, token).ConfigureAwait(false);
                if (processingResult)
                    return Ok("Excel File validated and pushed successfully to S3 for further processing, the processing will take few mins to complete");
                else
                    return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "File validattion failed and not pushed to S3"));
            }
            else
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, ModelState.GetErrorMessages()));
            }
        }

        /// <summary>
        /// Gets latest file processing logs.
        /// </summary>
        /// <param name="useCase">The use case.</param>
        /// <response code="200">List of processed files</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(List<FileProcessingLogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet("file-processing-logs")]
        [LogCall(LogLevel.Information)]
        public async Task<ActionResult<List<FileProcessingLogResponse>>> GetAllFileProcessingLogsAsync([FromServices] IGetFileProcessingLogUseCase useCase)
        {
            var response = await useCase.ExecuteAsync(Constants.EstimateUpload).ConfigureAwait(false);
            return Ok(response);
        }
    }
}
