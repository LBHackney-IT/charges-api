using AutoFixture;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Controllers;
using ChargesApi.V1.Domain;
using ChargesApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class EstimatesUploadControllerTests
    {
        private readonly EstimatesActualUploadController _estimatesUploadController;
        private readonly ControllerContext _controllerContext;
        private readonly HttpContext _httpContext;
        private readonly Fixture _fixture;

        private readonly Mock<IEstimateActualUploadUseCase> _mockAddEstimateChargesUseCase;

        public EstimatesUploadControllerTests()
        {
            _fixture = new Fixture();
            _mockAddEstimateChargesUseCase = new Mock<IEstimateActualUploadUseCase>();
            _httpContext = new DefaultHttpContext();
            _controllerContext = new ControllerContext(new ActionContext(_httpContext, new RouteData(), new ControllerActionDescriptor()));
            _estimatesUploadController = new EstimatesActualUploadController(_mockAddEstimateChargesUseCase.Object)
            {
                ControllerContext = _controllerContext
            };
        }

        //[Fact]
        //public async Task AddValidFileAndValidChargeGroupReturnsOkResult()
        //{
        //    _mockAddEstimateChargesUseCase.Setup(_ => _.AddEstimates(It.IsAny<IFormFile>(), It.IsAny<ChargeGroup>(),
        //        It.IsAny<string>())).Returns(Task.FromResult(9466));

        //    using var sourceFile = File.OpenRead(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.Substring(0, Assembly.GetEntryAssembly().Location.IndexOf("bin\\"))), "EstimatesTest.xlsx"));
        //    using var stream = sourceFile;

        //    var file = new FormFile(stream, 0, stream.Length, null, "EstimatesTest.xlsx")
        //    {
        //        Headers = new HeaderDictionary(),
        //        ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        //    };
        //    var request = new AddEstimatesRequest { ChargeGroup = ChargeGroup.Leaseholders, EstimatesFile = file };
        //    //var file = _fixture.Build<AddEstimatesRequest>().Create();
        //    var result = await _estimatesUploadController.Post(Token, request)
        //        .ConfigureAwait(false);

        //    result.Should().NotBeNull();

        //    var okResult = result as OkObjectResult;

        //    okResult.Should().NotBeNull();

        //    okResult.Value.Should().Be("9466 estimates records processed successfully");
        //}
        //[Fact]
        //public async Task InvalidFileAndInvalidModelReturnsBadRequest()
        //{

        //    var result = await _estimatesUploadController.Post(Token, null)
        //         .ConfigureAwait(false);

        //    result.Should().NotBeNull();

        //    var okResult = result as BadRequestObjectResult;

        //    okResult.Should().NotBeNull();
        //}
    }
}
