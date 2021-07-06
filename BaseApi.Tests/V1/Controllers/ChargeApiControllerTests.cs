using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Controllers;
using ChargeApi.V1.Domain;
using ChargeApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ChargeApi.Tests.V1.Controllers
{
    public class ChargeApiControllerTests
    {
        private readonly ChargeApiController _chargeController;
        private readonly ControllerContext _controllerContext;
        private readonly HttpContext _httpContext;

        private readonly Mock<IGetAllUseCase> _getAllUseCase;
        private readonly Mock<IGetByIdUseCase> _getByIdUseCase;
        private readonly Mock<IAddUseCase> _addUseCase;
        private readonly Mock<IRemoveUseCase> _removeUseCase;
        private readonly Mock<IUpdateUseCase> _updateUseCase;

        public ChargeApiControllerTests()
        {
            _getAllUseCase = new Mock<IGetAllUseCase>();
            _getByIdUseCase = new Mock<IGetByIdUseCase>();
            _addUseCase = new Mock<IAddUseCase>();
            _removeUseCase = new Mock<IRemoveUseCase>();
            _updateUseCase = new Mock<IUpdateUseCase>();

            _httpContext = new DefaultHttpContext();
            _controllerContext = new ControllerContext(new ActionContext(_httpContext, new RouteData(), new ControllerActionDescriptor()));
            _chargeController = new ChargeApiController(_getAllUseCase.Object, _getByIdUseCase.Object, _addUseCase.Object,
                                                        _removeUseCase.Object, _updateUseCase.Object)
            {
                ControllerContext = _controllerContext
            };
        }

        [Fact]
        public async Task GetAllByTypeAndTargetIdReturns200()
        {
            _getAllUseCase.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<ChargeResponse>()
                    {
                        new ChargeResponse
                        {
                            Id = new Guid("271b9a38-e78f-4a3f-81c0-4541bc5acc2c"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                            TargetType = TargetType.Asset,
                            DetailedCharges = new List<DetailedCharges>()
                        },
                        new ChargeResponse
                        {
                            Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                            TargetType = TargetType.Asset,
                            DetailedCharges = new List<DetailedCharges>()
                            {
                                new DetailedCharges
                                {
                                    Type = "Type",
                                    SubType = "SubType",
                                    StartDate = new DateTime(2021, 7, 2),
                                    EndDate = new DateTime(2021, 7, 4),
                                    Amount = 150,
                                    Frequency = "Frequency"
                                }
                            }
                        }
                    });

            var result = await _chargeController.GetAllAsync("Asset", new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"))
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();

            var charges = okResult.Value as List<ChargeResponse>;

            charges.Should().NotBeNull();

            charges.Should().HaveCount(2);

            charges[0].Id.Should().Be(new Guid("271b9a38-e78f-4a3f-81c0-4541bc5acc2c"));
            charges[0].TargetId.Should().Be(new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"));
            charges[0].TargetType.Should().Be(TargetType.Asset);
            charges[0].DetailedCharges.Should().NotBeNull();
            charges[0].DetailedCharges.Should().BeEmpty();

            charges[1].Id.Should().Be(new Guid("0f668265-1501-4722-8e37-77c7116dae2f"));
            charges[1].TargetId.Should().Be(new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"));
            charges[1].TargetType.Should().Be(TargetType.Asset);
            charges[1].DetailedCharges.Should().NotBeNull();
            charges[1].DetailedCharges.Should().HaveCount(1);

            var detailedChargesSecondElement = charges[1].DetailedCharges.ToList();
            detailedChargesSecondElement[0].Type.Should().Be("Type");
            detailedChargesSecondElement[0].SubType.Should().Be("SubType");
            detailedChargesSecondElement[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            detailedChargesSecondElement[0].EndDate.Should().Be(new DateTime(2021, 7, 4));
            detailedChargesSecondElement[0].Amount.Should().Be(150);
            detailedChargesSecondElement[0].Frequency.Should().Be("Frequency");
        }
    }
}
