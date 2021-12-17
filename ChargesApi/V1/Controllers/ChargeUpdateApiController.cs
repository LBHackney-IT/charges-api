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
    [Route("api/v1/charges-update")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ChargeUpdateApiController : BaseController
    {
        private readonly IAddChargesUpdateUseCase _addChargesUpdateUseCase;

        public ChargeUpdateApiController(IAddChargesUpdateUseCase addChargesUpdateUseCase)
        {
            _addChargesUpdateUseCase = addChargesUpdateUseCase;
        }
        /// <summary>
        /// Create new Charges Update event in Charges SNS TOPIC for Block / Estate
        /// </summary>
        /// <param name="chargesUpdate">Charges Update model for create event</param>
        /// <response code="200">Success. Charges Update event was created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargesUpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Post(AddChargesUpdateRequest chargesUpdate)
        {
            if (chargesUpdate == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "ChargesUpdate model cannot be null!"));
            }

            if (ModelState.IsValid)
            {
                var chargesUpdateResponse = await _addChargesUpdateUseCase.ExecuteAsync(chargesUpdate).ConfigureAwait(false);

                return Ok(chargesUpdateResponse);
            }
            else
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, ModelState.GetErrorMessages()));
            }
        }
    }
}
