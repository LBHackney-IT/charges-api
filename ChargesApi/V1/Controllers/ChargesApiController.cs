using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ChargesApi.V1.Infrastructure;
using Microsoft.AspNetCore.JsonPatch;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Infrastructure.Validators;
using FluentValidation.Results;

namespace ChargesApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/charges")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ChargesApiController : BaseController
    {
        private readonly IGetAllUseCase _getAllUseCase;
        private readonly IGetByIdUseCase _getByIdUseCase;
        private readonly IAddUseCase _addUseCase;
        private readonly IRemoveUseCase _removeUseCase;
        private readonly IUpdateUseCase _updateUseCase;
        private readonly IAddBatchUseCase _addBatchUseCase;
        private readonly IUpdateChargeUseCase _updateChargeUseCase;

        public ChargesApiController(
            IGetAllUseCase getAllUseCase,
            IGetByIdUseCase getByIdUseCase,
            IAddUseCase addUseCase,
            IRemoveUseCase removeUseCase,
            IUpdateUseCase updateUseCase,
            IAddBatchUseCase addBatchUseCase,
            IUpdateChargeUseCase updateChargeUseCase
        )
        {
            _getAllUseCase = getAllUseCase;
            _getByIdUseCase = getByIdUseCase;
            _addUseCase = addUseCase;
            _removeUseCase = removeUseCase;
            _updateUseCase = updateUseCase;
            _addBatchUseCase = addBatchUseCase;
            _updateChargeUseCase = updateChargeUseCase;
        }

        /// <summary>
        /// Get Charge model by provided id
        /// </summary>
        /// <param name="id">The value by which we are looking for charge</param>
        /// <param name="targetId">The value by which we are looking for charge</param>
        /// <response code="200">Success. Charge model was received successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Charge with provided id cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id, [FromQuery] Guid targetId)
        {
            var charge = await _getByIdUseCase.ExecuteAsync(id, targetId).ConfigureAwait(false);

            if (charge == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No Charge by provided Id cannot be found!"));
            }

            return Ok(charge);
        }

        /// <summary>
        /// Get a list of charge models by provided type and targetId
        /// </summary>
        /// <param name="targetId">Id of the appropriate tenure</param>
        /// <response code="200">Success. Charge models was received successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Charges with provided id cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(List<ChargeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] Guid targetId)
        {
            var charges = await _getAllUseCase.ExecuteAsync(targetId).ConfigureAwait(false);

            if (charges == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No Charges by provided type and targetId cannot be found!"));
            }

            return Ok(charges);
        }

        /// <summary>
        /// Create new Charge model
        /// </summary>
        /// <param name="correlationId">The value that is used to combine several requests into a common group</param>
        /// <param name="token">The jwt token value</param>
        /// <param name="charge">Charge model for create</param>
        /// <response code="201">Success. Charge model was created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargeResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Post([FromHeader(Name = "x-correlation-id")] string correlationId,
                                             [FromHeader(Name = "Authorization")] string token,
                                             [FromBody] AddChargeRequest charge)
        {
            if (charge == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Charge model cannot be null!"));
            }

            if (ModelState.IsValid)
            {
                var chargeResponse = await _addUseCase.ExecuteAsync(charge, token).ConfigureAwait(false);

                return CreatedAtAction($"Get", new { id = chargeResponse.Id, targetId = chargeResponse.TargetId }, chargeResponse);
            }
            else
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, ModelState.GetErrorMessages()));
            }
        }

        /// <summary>
        /// Create a list of new charges records
        /// </summary>
        /// <param name="correlationId">The value that is used to combine several requests into a common group</param>
        /// <param name="token">The jwt token value</param>
        /// <param name="charges">List of Charges model for create</param>
        /// <response code="201">Created. Charges model was created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("process-batch")]
        public async Task<IActionResult> AddBatch([FromHeader(Name = "x-correlation-id")] string correlationId,
                                                  [FromHeader(Name = "Authorization")] string token,
                                                  [FromBody] IEnumerable<AddChargeRequest> charges)
        {
            if (charges == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Charges models cannot be null!"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, GetErrorMessage(ModelState)));
            }

            var batchResponse = await _addBatchUseCase.ExecuteAsync(charges).ConfigureAwait(false);

            if (batchResponse == charges.Count())
                return Ok($"Total {batchResponse} number of Charges processed successfully");

            return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Charges entries processing failed!"));
        }

        /// <summary>
        /// Update existing charge model
        /// </summary>
        /// <param name="correlationId">The value that is used to combine several requests into a common group</param>
        /// <param name="token">The jwt token value</param>
        /// <param name="id">The value by which we are looking for charge</param>
        /// <param name="targetId">The value by which we are looking for charge</param>
        /// <param name="patchDoc">Charge model for update</param>
        /// <response code="200">Success. Charge models was updated successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Charge with provided id cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [Route("{id}")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromHeader(Name = "x-correlation-id")] string correlationId,
                                             [FromHeader(Name = "Authorization")] string token,
                                             [FromRoute] Guid id, [FromQuery] Guid targetId,
                                             [FromBody] JsonPatchDocument<UpdateChargeRequest> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Charge model cannot be null!"));
            }

            ChargeResponse chargeResponseObject = await _getByIdUseCase.ExecuteAsync(id, targetId).ConfigureAwait(false);

            if (chargeResponseObject == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No Charge by Id cannot be found!"));
            }
            UpdateChargeRequest updateCharge = chargeResponseObject.ToUpdateModel();

            patchDoc.ApplyTo(updateCharge);

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, ModelState.GetErrorMessages()));
            }

            UpdateChargeRequestValidator validator = new UpdateChargeRequestValidator();
            ValidationResult updateModelValidation = validator.Validate(updateCharge);
            if (!updateModelValidation.IsValid)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, updateModelValidation.GetErrorMessages()));
            }

            chargeResponseObject = updateCharge.ToResponse();
            var response = await _updateUseCase.ExecuteAsync(chargeResponseObject, token).ConfigureAwait(false);

            return Ok(response);
        }

        /// <summary>
        /// Delete existing charge model
        /// </summary>
        /// <param name="id">The value by which we are looking for charge</param>
        /// <param name="targetId">The value by which we are looking for charge</param>
        /// <response code="204">Success. Charge models was deleted successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Charge with provided id cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromQuery] Guid targetId)
        {
            var response = await _removeUseCase.ExecuteAsync(id, targetId).ConfigureAwait(false);
            if (!response)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No Charge by Id cannot be found!"));
            }

            return NoContent();
        }

        /// <summary>
        /// Update existing charge model
        /// </summary>
        /// <param name="token">The jwt token value</param>
        /// <param name="targetId">The value by which we are looking for charge</param>
        /// <param name="chargesUpdateRequest">Charge model for update</param>
        /// <response code="200">Success. Charge models was updated successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Charge with provided id cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut]
        public async Task<IActionResult> Update([FromHeader(Name = "Authorization")] string token, [FromQuery] Guid targetId,
                                             [FromBody] ChargesUpdateRequest chargesUpdateRequest)
        {
            if (chargesUpdateRequest == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Charge model cannot be null!"));
            }

            await _updateChargeUseCase.ExecuteAsync(targetId, chargesUpdateRequest.ToDomain(), token).ConfigureAwait(false);
            return Ok();
        }
    }
}
