using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Domain;
using ChargeApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ChargeApi.V1.Factories;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ChargeApi.V1.Controllers
{
    [ApiController]
    //TODO: Rename to match the APIs endpoint
    [Route("api/v1/charges")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    //TODO: rename class to match the API name
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

        [ProducesResponseType(typeof(ChargeResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var charge = await _getByIdUseCase.ExecuteAsync(id).ConfigureAwait(false);
                if (charge == null)
                    return NotFound(id);
                return Ok(charge);
            }
            catch(FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(typeof(ChargeResponseObjectList), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery]string type,[FromQuery] Guid targetid)
        {
            try
            {
                var charges = await _getAllUseCase.ExecuteAsync(type, targetid).ConfigureAwait(false);
                if (charges == null)
                    return NotFound(targetid);
                return Ok(charges);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(typeof(ChargeResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Post(Charge charge)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _addUseCase.ExecuteAsync(charge).ConfigureAwait(false);

                    return RedirectToAction("Get", new {id = charge.Id});
                }
                else
                {
                    return BadRequest(ModelState.Values.First().Errors[0].ErrorMessage);
                }
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(typeof(ChargeResponseObject),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] Charge charge)
        {
            try
            {
                if (id != charge.Id)
                    return BadRequest(id);

                ChargeResponseObject chargeResponseObject = await _getByIdUseCase.ExecuteAsync(id).ConfigureAwait(false);
                if (chargeResponseObject == null)
                    return NotFound(id);

                await _updateUseCase.ExecuteAsync(charge).ConfigureAwait(false);
                return RedirectToAction("Get", new { id = charge.Id });
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                ChargeResponseObject charge = await _getByIdUseCase.ExecuteAsync(id).ConfigureAwait(false);
                if (charge == null)
                    return NotFound(id);
                await _removeUseCase.ExecuteAsync(id).ConfigureAwait(false);
                return NoContent();
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
