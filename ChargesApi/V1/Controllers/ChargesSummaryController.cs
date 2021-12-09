using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ChargesApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/charges-summary")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ChargesSummaryController : BaseController
    {
        private readonly IGetChargesSummaryUseCase _getChargesSummaryUseCase;

        public ChargesSummaryController(IGetChargesSummaryUseCase getChargesSummaryUseCase)
        {
            _getChargesSummaryUseCase = getChargesSummaryUseCase;
        }

        /// <summary>
        /// Get All Charges Summary model by provided estate or block Id
        /// </summary>
        /// <param name="targetId">The value by which we are looking for charges summary</param>
        /// <response code="200">Success. Charges Summary model was received successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Charges Summary with provided id cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargesSummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid targetId)
        {
            var chargesList = await _getChargesSummaryUseCase.ExecuteAsync(targetId).ConfigureAwait(false);

            if (chargesList == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No ChargesSummary by provided Id cannot be found!"));
            }

            return Ok(chargesList);
        }
    }
}
