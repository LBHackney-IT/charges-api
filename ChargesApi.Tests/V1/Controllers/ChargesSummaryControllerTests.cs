using AutoFixture;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Controllers;
using ChargesApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.Controllers
{
    public class ChargesSummaryControllerTests
    {
        private readonly ChargesSummaryController _chargesSummaryController;
        private readonly ControllerContext _controllerContext;
        private readonly HttpContext _httpContext;
        private readonly Mock<IGetChargesSummaryUseCase> _getChargesSummaryUseCase;
        private readonly Fixture _fixture = new Fixture();
        public ChargesSummaryControllerTests()
        {
            _getChargesSummaryUseCase = new Mock<IGetChargesSummaryUseCase>();
            _httpContext = new DefaultHttpContext();
            _controllerContext = new ControllerContext(new ActionContext(_httpContext, new RouteData(), new ControllerActionDescriptor()));
            _chargesSummaryController = new ChargesSummaryController(_getChargesSummaryUseCase.Object)
            {
                ControllerContext = _controllerContext
            };
        }
        [Fact]
        public async Task GetAllChargesListReturns200()
        {
            var response = _fixture.Create<ChargesSummaryResponse>();
            _getChargesSummaryUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(response);

            var result = await _chargesSummaryController.GetAll(new Guid("84613e2b-b10d-4c09-910b-8375ba2d6aa7"), "block")
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();

            var chargeSummaryResponse = okResult.Value as ChargesSummaryResponse;

            chargeSummaryResponse.Should().NotBeNull();

            chargeSummaryResponse.TargetId.Should().Be(response.TargetId);
            chargeSummaryResponse.TargetType.Should().Be(response.TargetType);
            chargeSummaryResponse.ChargesList.Should().BeEquivalentTo(response.ChargesList);

        }
        [Fact]
        public async Task GetAllInvalidIdReturns404()
        {
            ChargesSummaryResponse expectedResponse = null;
            _getChargesSummaryUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);

            var result = await _chargesSummaryController.GetAll(new Guid("84613e2b-b10d-4c09-910b-8375ba2d6aa7"), "block")
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var notFoundResult = result as NotFoundObjectResult;

            notFoundResult.Should().NotBeNull();

            var response = notFoundResult.Value as BaseErrorResponse;

            response.Should().NotBeNull();

            response.StatusCode.Should().Be(404);
            response.Message.Should().BeEquivalentTo("No ChargesSummary by provided Id cannot be found!");
            response.Details.Should().BeEquivalentTo("");
        }
        [Fact]
        public async Task GetAllReturns500()
        {
            _getChargesSummaryUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"));

            try
            {
                var result = await _chargesSummaryController.GetAll(new Guid("84613e2b-b10d-4c09-910b-8375ba2d6aa7"), "block")
                    .ConfigureAwait(false);
                Assert.True(false, "It should return exception, not come this");
            }
            catch (Exception ex)
            {
                ex.Should().BeOfType(typeof(Exception));
                ex.Message.Should().BeEquivalentTo("Test exception");
            }
        }
    }
}
