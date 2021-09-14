using AutoFixture;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Controllers;
using ChargesApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.Controllers
{
    public class ChargeUpdateApiControllerTests
    {
        private readonly ChargeUpdateApiController _chargeUpdateApiController;
        private readonly ControllerContext _controllerContext;
        private readonly HttpContext _httpContext;
        private readonly Mock<IAddChargesUpdateUseCase> _addChargesUpdateUseCase;
        private readonly Fixture _fixture = new Fixture();
        public ChargeUpdateApiControllerTests()
        {
            _addChargesUpdateUseCase = new Mock<IAddChargesUpdateUseCase>();

            _httpContext = new DefaultHttpContext();
            _controllerContext = new ControllerContext(new ActionContext(_httpContext, new RouteData(), new ControllerActionDescriptor()));
            _chargeUpdateApiController = new ChargeUpdateApiController(_addChargesUpdateUseCase.Object)
            {
                ControllerContext = _controllerContext
            };
        }
        [Fact]
        public async Task AddChargeUpdateWithValidDataReturns200()
        {
            var response = _fixture.Create<ChargesUpdateResponse>();
            _addChargesUpdateUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<AddChargesUpdateRequest>())).ReturnsAsync(response);

            var request = _fixture.Create<AddChargesUpdateRequest>();
            var result = await _chargeUpdateApiController.Post(request).ConfigureAwait(false);

            result.Should().NotBeNull();

            var chargesUpdateResponse = result as OkObjectResult;

            chargesUpdateResponse.Should().NotBeNull();

            chargesUpdateResponse.Value.Should().NotBeNull();

            var chargeResponse = chargesUpdateResponse.Value as ChargesUpdateResponse;

            chargeResponse.Should().NotBeNull();
        }
        [Fact]
        public async Task AddChargeUpdateWithNullReturns400()
        {
            var result = await _chargeUpdateApiController.Post(null).ConfigureAwait(false);

            var badRequest = result as BadRequestObjectResult;

            badRequest.Should().NotBeNull();

            var error = badRequest.Value as BaseErrorResponse;

            error.Should().NotBeNull();

            error.StatusCode.Should().Be(400);

            error.Message.Should().BeEquivalentTo("ChargesUpdate model cannot be null!");

            error.Details.Should().BeEquivalentTo("");
        }
    }
}
