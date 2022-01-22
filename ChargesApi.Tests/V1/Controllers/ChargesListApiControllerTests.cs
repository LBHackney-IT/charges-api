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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.Controllers
{
    public class ChargesListApiControllerTests
    {
        private readonly ChargesListApiController _chargesListApiController;
        private readonly ControllerContext _controllerContext;
        private readonly HttpContext _httpContext;

        private readonly Mock<IAddChargesListUseCase> _addChargesListUseCase;
        private readonly Mock<IGetAllChargesListUseCase> _getAllChargesListUseCase;
        private readonly Mock<IGetByIdChargesListUseCase> _getByIdChargesListUseCase;
        private readonly Fixture _fixture = new Fixture();
        private const string Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ0ZXN0IiwiaWF0IjoxNjM5NDIyNzE4LCJleHAiOjE5ODY1Nzc5MTgsImF1ZCI6InRlc3QiLCJzdWIiOiJ0ZXN0IiwiZ3JvdXBzIjpbInNvbWUtdmFsaWQtZ29vZ2xlLWdyb3VwIiwic29tZS1vdGhlci12YWxpZC1nb29nbGUtZ3JvdXAiXSwibmFtZSI6InRlc3RpbmcifQ.IcpQ00PGVgksXkR_HFqWOakgbQ_PwW9dTVQu4w77tmU";


        public ChargesListApiControllerTests()
        {
            _addChargesListUseCase = new Mock<IAddChargesListUseCase>();
            _getAllChargesListUseCase = new Mock<IGetAllChargesListUseCase>();
            _getByIdChargesListUseCase = new Mock<IGetByIdChargesListUseCase>();

            _httpContext = new DefaultHttpContext();
            _controllerContext = new ControllerContext(new ActionContext(_httpContext, new RouteData(), new ControllerActionDescriptor()));
            _chargesListApiController = new ChargesListApiController(_addChargesListUseCase.Object,
                _getAllChargesListUseCase.Object, _getByIdChargesListUseCase.Object)
            {
                ControllerContext = _controllerContext
            };
        }
        [Fact]
        public async Task AddChargeListWithValidDataReturns201()
        {
            var response = _fixture.Create<List<ChargesListResponse>>();
            _getAllChargesListUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<string>())).ReturnsAsync(response);

            var addResponse = _fixture.Create<ChargesListResponse>();
            _addChargesListUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<AddChargesListRequest>(), It.IsAny<string>())).ReturnsAsync(addResponse);
            var request = _fixture.Create<AddChargesListRequest>();
            var result = await _chargesListApiController.Post(Guid.NewGuid().ToString(), Token, request).ConfigureAwait(false);

            result.Should().NotBeNull();

            var createdAtActionResult = result as CreatedAtActionResult;

            createdAtActionResult.Should().NotBeNull();

            createdAtActionResult.ActionName.Should().BeEquivalentTo("Get");

            createdAtActionResult.RouteValues["id"].Should().NotBeNull();

            createdAtActionResult.RouteValues["id"].Should().BeOfType(typeof(Guid));

            createdAtActionResult.Value.Should().NotBeNull();

            var chargeResponse = createdAtActionResult.Value as ChargesListResponse;

            chargeResponse.Should().NotBeNull();

        }
        [Fact]
        public async Task AddChargeListWithNullReturns400()
        {
            var result = await _chargesListApiController.Post(Guid.NewGuid().ToString(), Token, null).ConfigureAwait(false);

            var badRequest = result as BadRequestObjectResult;

            badRequest.Should().NotBeNull();

            var error = badRequest.Value as BaseErrorResponse;

            error.Should().NotBeNull();

            error.StatusCode.Should().Be(400);

            error.Message.Should().BeEquivalentTo("ChargesList model cannot be null!");

            error.Details.Should().BeEquivalentTo("");
        }
        [Fact]
        public async Task GetByIdValidIdReturns200()
        {
            var addResponse = _fixture.Create<ChargesListResponse>();
            _getByIdChargesListUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(addResponse);

            var result = await _chargesListApiController.Get(new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"),
                "DCB")
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();

            var chargeResponse = okResult.Value as ChargesListResponse;

            chargeResponse.Should().NotBeNull();

            chargeResponse.Id.Should().Be(addResponse.Id);
            chargeResponse.ChargeCode.Should().Be(addResponse.ChargeCode);
            chargeResponse.ChargeGroup.Should().Be(addResponse.ChargeGroup);
            chargeResponse.ChargeName.Should().Be(addResponse.ChargeName);
            chargeResponse.ChargeType.Should().Be(addResponse.ChargeType);

        }
        [Fact]
        public async Task GetByIdInvalidIdReturns404()
        {
            _getByIdChargesListUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync((ChargesListResponse) null);

            var result = await _chargesListApiController.Get(new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"),
                "DCB")
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var notFoundResult = result as NotFoundObjectResult;

            notFoundResult.Should().NotBeNull();

            var response = notFoundResult.Value as BaseErrorResponse;

            response.Should().NotBeNull();

            response.StatusCode.Should().Be(404);
            response.Message.Should().BeEquivalentTo("No ChargesList by provided Id cannot be found!");
            response.Details.Should().BeEquivalentTo("");
        }
        [Fact]
        public async Task GetByIdReturns500()
        {
            _getByIdChargesListUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"));

            try
            {
                var result = await _chargesListApiController.Get(new Guid("b45d2bbf-abec-454c-a843-4667786177a1"),
                    "DCB")
                    .ConfigureAwait(false);
                Assert.True(false, "It should return exception, not come this");
            }
            catch (Exception ex)
            {
                ex.Should().BeOfType(typeof(Exception));
                ex.Message.Should().BeEquivalentTo("Test exception");
            }
        }
        [Fact]
        public async Task GetAllChargesListReturns200()
        {
            var response = _fixture.Create<List<ChargesListResponse>>();
            _getAllChargesListUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<string>())).ReturnsAsync(response);

            var result = await _chargesListApiController.GetAll("DCB").ConfigureAwait(false);
            result.Should().NotBeNull();

            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();

            var chargeResponse = okResult.Value as List<ChargesListResponse>;

            chargeResponse.Should().NotBeNull();

            chargeResponse[0].Id.Should().Be(response[0].Id);
            chargeResponse[0].ChargeCode.Should().Be(response[0].ChargeCode);
            chargeResponse[0].ChargeGroup.Should().Be(response[0].ChargeGroup);
            chargeResponse[0].ChargeName.Should().Be(response[0].ChargeName);
            chargeResponse[0].ChargeType.Should().Be(response[0].ChargeType);

        }
        [Fact]
        public async Task GetAllInvalidIdReturns404()
        {
            List<ChargesListResponse> expectedResponse = null;
            _getAllChargesListUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);

            var result = await _chargesListApiController.GetAll("DCB")
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var notFoundResult = result as NotFoundObjectResult;

            notFoundResult.Should().NotBeNull();

            var response = notFoundResult.Value as BaseErrorResponse;

            response.Should().NotBeNull();

            response.StatusCode.Should().Be(404);
            response.Message.Should().BeEquivalentTo("No ChargesList by provided Id cannot be found!");
            response.Details.Should().BeEquivalentTo("");
        }
        [Fact]
        public async Task GetAllReturns500()
        {
            _getAllChargesListUseCase.Setup(x => x.ExecuteAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"));

            try
            {
                var result = await _chargesListApiController.GetAll("DCB")
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
