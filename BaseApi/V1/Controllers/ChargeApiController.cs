using ChargeApi.V1.Boundary.Request;
using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ChargeApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/charges")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ChargeApiController : BaseController
    {
        private readonly IGetAllUseCase _getAllUseCase;
        private readonly IGetByIdUseCase _getByIdUseCase;
        private readonly IAddUseCase _addUseCase;
        private readonly IRemoveUseCase _removeUseCase;
        private readonly IUpdateUseCase _updateUseCase;

        public ChargeApiController(
            IGetAllUseCase getAllUseCase,
            IGetByIdUseCase getByIdUseCase,
            IAddUseCase addUseCase,
            IRemoveUseCase removeUseCase,
            IUpdateUseCase updateUseCase
        )
        {
            _getAllUseCase = getAllUseCase;
            _getByIdUseCase = getByIdUseCase;
            _addUseCase = addUseCase;
            _removeUseCase = removeUseCase;
            _updateUseCase = updateUseCase;
        }

        /// <summary>
        /// Get Charge model by provided id
        /// </summary>
        /// <param name="id">The value by which we are looking for charge</param>
        /// <response code="200">Success. Charge model was received successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Charge with provided id cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            if (id == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Id cannot be null!"));
            }

            var charge = await _getByIdUseCase.ExecuteAsync(id).ConfigureAwait(false);

            if (charge == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No Charge by provided id cannot be found!"));
            }

            return Ok(charge);
        }

        /// <summary>
        /// Get a list of charge models by provided type and targetId
        /// </summary>
        /// <param name="type">Type of charge</param>
        /// <param name="targetId">Id of the appropriate tenure</param>
        /// <response code="200">Success. Charge models was received successfully</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(List<ChargeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] string type, [FromQuery] Guid targetId)
        {
            var charges = await _getAllUseCase.ExecuteAsync(type, targetId).ConfigureAwait(false);

            return Ok(charges);
        }

        /// <summary>
        /// Create new Charge model
        /// </summary>
        /// <param name="charge">Charge model for creatr</param>
        /// <response code="200">Success. Charge models was created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddChargeRequest charge)
        {
            if(charge == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Charge model cannot be null."));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, GetErrorMessage(ModelState)));
            }

            var result = await _addUseCase.ExecuteAsync(charge).ConfigureAwait(false);

            return RedirectToAction("Get", new { id = result.Id });
        }

        /// <summary>
        /// Update existing charge model
        /// </summary>
        /// <param name="id">The value by which we are looking for charge</param>
        /// <param name="charge">Charge model for update</param>
        /// <response code="200">Success. Charge models was updated successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateChargeRequest charge)
        {
            if (charge == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Charge model cannot be null."));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, GetErrorMessage(ModelState)));
            }

            if (id != charge.Id)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Different Ids in model and route."));
            }

            ChargeResponse chargeEntity = await _getByIdUseCase.ExecuteAsync(id).ConfigureAwait(false);

            if (chargeEntity == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No Charge by provided id cannot be found!"));
            }

            await _updateUseCase.ExecuteAsync(charge).ConfigureAwait(false);

            return RedirectToAction("Get", new { id = charge.Id });
        }

        /// <summary>
        /// Delete existing charge model
        /// </summary>
        /// <param name="id">The value by which we are looking for charge</param>
        /// <response code="204">Success. Charge models was deleted successfully</response>
        /// <response code="404">Charge with provided id cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            ChargeResponse charge = await _getByIdUseCase.ExecuteAsync(id).ConfigureAwait(false);

            if (charge == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No Charge by provided id cannot be found!"));
            }

            await _removeUseCase.ExecuteAsync(id).ConfigureAwait(false);

            return NoContent();
        }
    }
}
