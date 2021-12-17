using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Infrastructure;
using ChargesApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ChargesApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/charges-list")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ChargesListApiController : BaseController
    {
        private readonly IAddChargesListUseCase _addChargesListUseCase;
        private readonly IGetAllChargesListUseCase _getAllChargesListUseCase;
        private readonly IGetByIdChargesListUseCase _getByIdChargesListUseCase;

        public ChargesListApiController(IAddChargesListUseCase addChargesListUseCase,
            IGetAllChargesListUseCase getAllChargesListUseCase,
            IGetByIdChargesListUseCase getByIdChargesListUseCase)
        {
            _addChargesListUseCase = addChargesListUseCase;
            _getAllChargesListUseCase = getAllChargesListUseCase;
            _getByIdChargesListUseCase = getByIdChargesListUseCase;
        }
        /// <summary>
        /// Get Charges List model by provided id
        /// </summary>
        /// <param name="id">The value by which we are looking for charges list</param>
        /// <response code="200">Success. Charges list model was received successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Charges list with provided id cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargesListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var chargesList = await _getByIdChargesListUseCase.ExecuteAsync(id).ConfigureAwait(false);

            if (chargesList == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No ChargesList by provided Id cannot be found!"));
            }

            return Ok(chargesList);
        }

        /// <summary>
        /// Get All Charges List model by provided chargeGroup and chargeType
        /// </summary>
        /// <param name="chargeGroup">The value by which we are looking for charges list</param>
        /// <param name="chargeType">The value by which we are looking for charges list</param>
        /// <response code="200">Success. Charges list model was received successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Charges list with provided id cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargesListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string chargeGroup, string chargeType)
        {
            var chargesList = await _getAllChargesListUseCase.ExecuteAsync(chargeGroup, chargeType).ConfigureAwait(false);

            if (chargesList == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No ChargesList by provided Id cannot be found!"));
            }

            return Ok(chargesList);
        }

        /// <summary>
        /// Create new Charges list model
        /// </summary>
        /// <param name="chargesList">Charges list model for create</param>
        /// <response code="201">Success. Charges list model was created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ChargesListResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Post(AddChargesListRequest chargesList)
        {
            if (chargesList == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "ChargesList model cannot be null!"));
            }

            if (ModelState.IsValid)
            {
                var charges = await _getAllChargesListUseCase.ExecuteAsync(chargesList.ChargeGroup.ToString(), chargesList.ChargeType.ToString()).ConfigureAwait(false);
                if (charges.Any())
                {
                    var checkCharge = charges.FirstOrDefault(x => x.ChargeName == chargesList.ChargeName && x.ChargeCode == chargesList.ChargeCode);
                    if (checkCharge != null)
                    {
                        return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "Charge already exists!"));
                    }
                }
                var chargesListResponse = await _addChargesListUseCase.ExecuteAsync(chargesList).ConfigureAwait(false);

                return CreatedAtAction($"Get", new { id = chargesListResponse.Id }, chargesListResponse);
            }
            else
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, ModelState.GetErrorMessages()));
            }
        }
    }
}
