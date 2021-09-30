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
    [Route("api/v1/actuals")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ActualsController : BaseController
    {
        private readonly IAddActualsUseCase _addActualsUseCase;

        public ActualsController(IAddActualsUseCase addActualsUseCase)
        {
            _addActualsUseCase = addActualsUseCase;
        }

        /// <summary>
        /// Create new List of Actuals records by batch processing
        /// </summary>
        /// <param name="addActualsRequest">Actuals File model for create</param>
        /// <response code="201">Success. Actuals reccords was created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] AddActualsRequest addActualsRequest)
        {
            if (addActualsRequest == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Add Actuals request cannot be null!"));
            }
            if (ModelState.IsValid)
            {
                int processingCount = 0;
                if (addActualsRequest != null)
                {
                    processingCount = await _addActualsUseCase.AddActuals(addActualsRequest.ActualsFile).ConfigureAwait(false);
                }
                return Ok($"{processingCount} Actuals records processed successfully");
            }
            else
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, ModelState.GetErrorMessages()));
            }
        }

    }
}
