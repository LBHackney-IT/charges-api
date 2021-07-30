using ChargeApi.V1.Boundary.Request;
using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Infrastructure;
using ChargeApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ChargeApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/charges-maintenance")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ChargeMaintenanceApiController : BaseController
    {
        private readonly IAddChargeMaintenanceUseCase _addChargeMaintenanceUseCase;
        private readonly IGetByIdChargeMaintenanceUseCase _getByIdChargeMaintenanceUseCase;
        private readonly IGetByIdUseCase _getByIdUseCase;

        public ChargeMaintenanceApiController(
            IAddChargeMaintenanceUseCase addChargeMaintenanceUseCase,
            IGetByIdChargeMaintenanceUseCase getByIdChargeMaintenanceUseCase,
            IGetByIdUseCase getByIdUseCase)
        {
            _addChargeMaintenanceUseCase = addChargeMaintenanceUseCase;
            _getByIdChargeMaintenanceUseCase = getByIdChargeMaintenanceUseCase;
            _getByIdUseCase = getByIdUseCase;
        }

        /// <summary>
        /// Get Charge Maintenance model by provided id
        /// </summary>
        /// <param name="id">The value by which we are looking for charge maintenance</param>
        /// <response code="200">Success. Charge maintenance model was received successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Charge maintenance with provided id cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargeMaintenanceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var charge = await _getByIdChargeMaintenanceUseCase.ExecuteAsync(id).ConfigureAwait(false);

            if (charge == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No Charge Maintenance by provided Id cannot be found!"));
            }

            return Ok(charge);
        }

        /// <summary>
        /// Create new Charge maintenance model
        /// </summary>
        /// <param name="chargeMaintenance">Charge maintenance model for create</param>
        /// <response code="201">Success. Charge maintenance model was created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargeMaintenanceResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Post(AddChargeMaintenanceRequest chargeMaintenance)
        {
            if (chargeMaintenance == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Charge Maintenance model cannot be null!"));
            }

            if (ModelState.IsValid)
            {
                var checkChargeResult = await _getByIdUseCase.ExecuteAsync(chargeMaintenance.ChargesId).ConfigureAwait(false);
                if (checkChargeResult == null)
                {
                    return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "No Charge by provided ChargeId cannot be found!"));
                }
                var chargeMaintenanceResponse = await _addChargeMaintenanceUseCase.ExecuteAsync(chargeMaintenance).ConfigureAwait(false);

                return CreatedAtAction($"Get", new { id = chargeMaintenanceResponse.Id }, chargeMaintenanceResponse);
            }
            else
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, ModelState.GetErrorMessages()));
            }
        }
    }
}
