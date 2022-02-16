using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Controllers;
using ChargesApi.V1.Domain;
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
    public class ChargeMaintenanceApiControllerTests
    {
        private readonly ChargeMaintenanceApiController _chargeMaintenanceApiController;
        private readonly ControllerContext _controllerContext;
        private readonly HttpContext _httpContext;

        private readonly Mock<IAddChargeMaintenanceUseCase> _addChargeMaintenanceUseCase;
        private readonly Mock<IGetByIdUseCase> _getByIdUseCase;
        private readonly Mock<IGetByIdChargeMaintenanceUseCase> _getByIdChargeMaintenanceUseCase;

        public ChargeMaintenanceApiControllerTests()
        {
            _addChargeMaintenanceUseCase = new Mock<IAddChargeMaintenanceUseCase>();
            _getByIdUseCase = new Mock<IGetByIdUseCase>();
            _getByIdChargeMaintenanceUseCase = new Mock<IGetByIdChargeMaintenanceUseCase>();

            _httpContext = new DefaultHttpContext();
            _controllerContext = new ControllerContext(new ActionContext(_httpContext, new RouteData(), new ControllerActionDescriptor()));
            _chargeMaintenanceApiController = new ChargeMaintenanceApiController(_addChargeMaintenanceUseCase.Object,
                _getByIdChargeMaintenanceUseCase.Object, _getByIdUseCase.Object)
            {
                ControllerContext = _controllerContext
            };
        }
        [Fact]
        public async Task AddChargeMaintenanceWithValidDataReturns201()
        {
            _getByIdUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new ChargeResponse
            {
                Id = new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"),
                TargetId = new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"),
                TargetType = TargetType.Dwelling,
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
            });
            _addChargeMaintenanceUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<AddChargeMaintenanceRequest>()))
                .ReturnsAsync(new ChargeMaintenanceResponse
                {
                    Id = new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"),
                    ChargesId = new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"),
                    Reason = "Uplift",
                    NewValue = new List<DetailedCharges>()
                    {
                        new DetailedCharges
                        {
                            Type = "service",
                            SubType = "water",
                            StartDate = new DateTime(2021, 7, 2),
                            EndDate = new DateTime(2021, 7, 4),
                            Amount = 150,
                            Frequency = "weekly"
                        }
                    },
                    ExistingValue = new List<DetailedCharges>()
                    {
                        new DetailedCharges
                        {
                            Type = "service",
                            SubType = "water",
                            StartDate = new DateTime(2021, 7, 2),
                            EndDate = new DateTime(2021, 7, 4),
                            Amount = 120,
                            Frequency = "weekly"
                        }
                    },
                    StartDate = new DateTime(2021, 7, 2),
                    Status = ChargeMaintenanceStatus.Pending
                });

            var chargeMaintenance = new AddChargeMaintenanceRequest
            {
                ChargesId = new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"),
                TargetId = new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"),
                Reason = "Uplift",
                NewValue = new List<DetailedCharges>()
                    {
                        new DetailedCharges
                        {
                            Type = "service",
                            SubType = "water",
                            StartDate = new DateTime(2021, 7, 2),
                            EndDate = new DateTime(2021, 7, 4),
                            Amount = 150,
                            Frequency = "weekly"
                        }
                    },
                ExistingValue = new List<DetailedCharges>()
                    {
                        new DetailedCharges
                        {
                            Type = "service",
                            SubType = "water",
                            StartDate = new DateTime(2021, 7, 2),
                            EndDate = new DateTime(2021, 7, 4),
                            Amount = 120,
                            Frequency = "weekly"
                        }
                    },
                StartDate = new DateTime(2021, 7, 2),
                Status = ChargeMaintenanceStatus.Pending
            };


            var result = await _chargeMaintenanceApiController.Post(chargeMaintenance).ConfigureAwait(false);

            result.Should().NotBeNull();

            var createdAtActionResult = result as CreatedAtActionResult;

            createdAtActionResult.Should().NotBeNull();

            createdAtActionResult.ActionName.Should().BeEquivalentTo("Get");

            createdAtActionResult.RouteValues["id"].Should().NotBeNull();

            createdAtActionResult.RouteValues["id"].Should().BeOfType(typeof(Guid));

            createdAtActionResult.Value.Should().NotBeNull();

            var chargeResponse = createdAtActionResult.Value as ChargeMaintenanceResponse;

            chargeResponse.Should().NotBeNull();

            chargeResponse.Should().BeEquivalentTo(chargeMaintenance, opt => opt.Excluding(x => x.TargetId));
        }

        [Fact]
        public async Task AddChargeMaintenanceWithNullReturns400()
        {
            var result = await _chargeMaintenanceApiController.Post(null).ConfigureAwait(false);

            var badRequest = result as BadRequestObjectResult;

            badRequest.Should().NotBeNull();

            var error = badRequest.Value as BaseErrorResponse;

            error.Should().NotBeNull();

            error.StatusCode.Should().Be(400);

            error.Message.Should().BeEquivalentTo("Charge Maintenance model cannot be null!");

            error.Details.Should().BeEquivalentTo("");
        }

        [Fact]
        public async Task GetByIdValidIdReturns200()
        {
            _getByIdChargeMaintenanceUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ChargeMaintenanceResponse
                {
                    Id = new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"),
                    ChargesId = new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"),
                    Reason = "Uplift",
                    NewValue = new List<DetailedCharges>()
                    {
                        new DetailedCharges
                        {
                            Type = "service",
                            SubType = "water",
                            StartDate = new DateTime(2021, 7, 2),
                            EndDate = new DateTime(2021, 7, 4),
                            Amount = 150,
                            Frequency = "weekly"
                        }
                    },
                    ExistingValue = new List<DetailedCharges>()
                    {
                        new DetailedCharges
                        {
                            Type = "service",
                            SubType = "water",
                            StartDate = new DateTime(2021, 7, 2),
                            EndDate = new DateTime(2021, 7, 4),
                            Amount = 120,
                            Frequency = "weekly"
                        }
                    },
                    StartDate = new DateTime(2021, 7, 2),
                    Status = ChargeMaintenanceStatus.Pending
                });

            var result = await _chargeMaintenanceApiController.Get(new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"))
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();

            var chargeMaintenance = okResult.Value as ChargeMaintenanceResponse;

            chargeMaintenance.Should().NotBeNull();

            chargeMaintenance.Id.Should().Be(new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"));
            chargeMaintenance.ChargesId.Should().Be(new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"));
            chargeMaintenance.Status.Should().Be(ChargeMaintenanceStatus.Pending);
            chargeMaintenance.Reason.Should().Be("Uplift");
            chargeMaintenance.StartDate.Should().Be(new DateTime(2021, 7, 2));

            var chargeMaintenanceNewValue = chargeMaintenance.NewValue.ToList();
            chargeMaintenanceNewValue[0].Type.Should().Be("service");
            chargeMaintenanceNewValue[0].SubType.Should().Be("water");
            chargeMaintenanceNewValue[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            chargeMaintenanceNewValue[0].EndDate.Should().Be(new DateTime(2021, 7, 4));
            chargeMaintenanceNewValue[0].Amount.Should().Be(150);
            chargeMaintenanceNewValue[0].Frequency.Should().Be("weekly");

            var chargeMaintenanceExistingValue = chargeMaintenance.ExistingValue.ToList();
            chargeMaintenanceExistingValue[0].Type.Should().Be("service");
            chargeMaintenanceExistingValue[0].SubType.Should().Be("water");
            chargeMaintenanceExistingValue[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            chargeMaintenanceExistingValue[0].EndDate.Should().Be(new DateTime(2021, 7, 4));
            chargeMaintenanceExistingValue[0].Amount.Should().Be(120);
            chargeMaintenanceExistingValue[0].Frequency.Should().Be("weekly");
        }

        [Fact]
        public async Task GetByIdInvalidIdReturns404()
        {
            _getByIdChargeMaintenanceUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ChargeMaintenanceResponse) null);

            var result = await _chargeMaintenanceApiController.Get(new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"))
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var notFoundResult = result as NotFoundObjectResult;

            notFoundResult.Should().NotBeNull();

            var response = notFoundResult.Value as BaseErrorResponse;

            response.Should().NotBeNull();

            response.StatusCode.Should().Be(404);
            response.Message.Should().BeEquivalentTo("No Charge Maintenance by provided Id cannot be found!");
            response.Details.Should().BeEquivalentTo("");
        }

        [Fact]
        public async Task GetByIdReturns500()
        {
            _getByIdChargeMaintenanceUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Test exception"));

            try
            {
                var result = await _chargeMaintenanceApiController.Get(new Guid("b45d2bbf-abec-454c-a843-4667786177a1"))
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
