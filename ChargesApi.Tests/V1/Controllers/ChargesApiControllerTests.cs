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
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Xunit;
using Microsoft.AspNetCore.JsonPatch;

namespace ChargesApi.Tests.V1.Controllers
{
    public class ChargesApiControllerTests
    {
        private readonly ChargesApiController _chargeController;
        private readonly ControllerContext _controllerContext;
        private readonly HttpContext _httpContext;

        private readonly Mock<IGetAllUseCase> _getAllUseCase;
        private readonly Mock<IGetByIdUseCase> _getByIdUseCase;
        private readonly Mock<IAddUseCase> _addUseCase;
        private readonly Mock<IRemoveUseCase> _removeUseCase;
        private readonly Mock<IUpdateUseCase> _updateUseCase;
        private readonly Mock<IAddBatchUseCase> _batchUseCase;
        private const string Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ0ZXN0IiwiaWF0IjoxNjM5NDIyNzE4LCJleHAiOjE5ODY1Nzc5MTgsImF1ZCI6InRlc3QiLCJzdWIiOiJ0ZXN0IiwiZ3JvdXBzIjpbInNvbWUtdmFsaWQtZ29vZ2xlLWdyb3VwIiwic29tZS1vdGhlci12YWxpZC1nb29nbGUtZ3JvdXAiXSwibmFtZSI6InRlc3RpbmcifQ.IcpQ00PGVgksXkR_HFqWOakgbQ_PwW9dTVQu4w77tmU";

        public ChargesApiControllerTests()
        {
            _getAllUseCase = new Mock<IGetAllUseCase>();
            _getByIdUseCase = new Mock<IGetByIdUseCase>();
            _addUseCase = new Mock<IAddUseCase>();
            _removeUseCase = new Mock<IRemoveUseCase>();
            _updateUseCase = new Mock<IUpdateUseCase>();
            _batchUseCase = new Mock<IAddBatchUseCase>();

            _httpContext = new DefaultHttpContext();
            _controllerContext = new ControllerContext(new ActionContext(_httpContext, new RouteData(), new ControllerActionDescriptor()));
            _chargeController = new ChargesApiController(_getAllUseCase.Object, _getByIdUseCase.Object, _addUseCase.Object,
                                                        _removeUseCase.Object, _updateUseCase.Object, _batchUseCase.Object)
            {
                ControllerContext = _controllerContext
            };
        }

        [Fact]
        public async Task GetAllByTypeAndTargetIdReturns200()
        {
            _getAllUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ChargeResponse>()
                    {
                        new ChargeResponse
                        {
                            Id = new Guid("271b9a38-e78f-4a3f-81c0-4541bc5acc2c"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
                            TargetType = TargetType.Dwelling,
                            DetailedCharges = new List<DetailedCharges>()
                        },
                        new ChargeResponse
                        {
                            Id = new Guid("0f668265-1501-4722-8e37-77c7116dae2f"),
                            TargetId = new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"),
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
                        }
                    });

            var result = await _chargeController.GetAllAsync(new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"))
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();

            var charges = okResult.Value as List<ChargeResponse>;

            charges.Should().NotBeNull();

            charges.Should().HaveCount(2);

            charges[0].Id.Should().Be(new Guid("271b9a38-e78f-4a3f-81c0-4541bc5acc2c"));
            charges[0].TargetId.Should().Be(new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"));
            charges[0].TargetType.Should().Be(TargetType.Dwelling);
            charges[0].DetailedCharges.Should().NotBeNull();
            charges[0].DetailedCharges.Should().BeEmpty();

            charges[1].Id.Should().Be(new Guid("0f668265-1501-4722-8e37-77c7116dae2f"));
            charges[1].TargetId.Should().Be(new Guid("cb501c6e-b51c-47b4-9a7e-dddb8cb575ff"));
            charges[1].TargetType.Should().Be(TargetType.Dwelling);
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

        [Fact]
        public async Task GetAllByTypeAndAnotherTargetIdReturns200()
        {
            _getAllUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ChargeResponse>()
                    {
                        new ChargeResponse
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
                        }
                    });

            var result = await _chargeController.GetAllAsync(new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"))
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();

            var charges = okResult.Value as List<ChargeResponse>;

            charges.Should().NotBeNull();

            charges.Should().HaveCount(1);

            charges[0].Id.Should().Be(new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"));
            charges[0].TargetId.Should().Be(new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"));
            charges[0].TargetType.Should().Be(TargetType.Dwelling);
            charges[0].DetailedCharges.Should().NotBeNull();
            charges[0].DetailedCharges.Should().HaveCount(1);

            var detailedChargesSecondElement = charges[0].DetailedCharges.ToList();
            detailedChargesSecondElement[0].Type.Should().Be("Type");
            detailedChargesSecondElement[0].SubType.Should().Be("SubType");
            detailedChargesSecondElement[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            detailedChargesSecondElement[0].EndDate.Should().Be(new DateTime(2021, 7, 4));
            detailedChargesSecondElement[0].Amount.Should().Be(150);
            detailedChargesSecondElement[0].Frequency.Should().Be("Frequency");
        }

        [Fact]
        public async Task GetAllByTypeAndTargetIdReturns500()
        {
            _getAllUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Test exception"));

            try
            {
                var result = await _chargeController.GetAllAsync(new Guid("3687f3b1-0c50-4d5b-ad4d-bf2668cf5a11"))
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
        public async Task GetByIdValidIdReturns200()
        {
            _getByIdUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new ChargeResponse
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

            var result = await _chargeController.Get(new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"), new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"))
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();

            var charge = okResult.Value as ChargeResponse;

            charge.Should().NotBeNull();

            charge.Id.Should().Be(new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"));
            charge.TargetId.Should().Be(new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"));
            charge.TargetType.Should().Be(TargetType.Dwelling);
            charge.DetailedCharges.Should().NotBeNull();
            charge.DetailedCharges.Should().HaveCount(1);

            var detailedChargesSecondElement = charge.DetailedCharges.ToList();
            detailedChargesSecondElement[0].Type.Should().Be("Type");
            detailedChargesSecondElement[0].SubType.Should().Be("SubType");
            detailedChargesSecondElement[0].StartDate.Should().Be(new DateTime(2021, 7, 2));
            detailedChargesSecondElement[0].EndDate.Should().Be(new DateTime(2021, 7, 4));
            detailedChargesSecondElement[0].Amount.Should().Be(150);
            detailedChargesSecondElement[0].Frequency.Should().Be("Frequency");
        }

        [Fact]
        public async Task GetByIdInvalidIdReturns404()
        {
            _getByIdUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((ChargeResponse) null);

            var result = await _chargeController.Get(new Guid("a93e7d88-5074-4c50-b51a-b35292545ffb"), new Guid("a93e7d88-5074-4c50-b51a-b35292545ffb"))
                .ConfigureAwait(false);

            result.Should().NotBeNull();

            var notFoundResult = result as NotFoundObjectResult;

            notFoundResult.Should().NotBeNull();

            var response = notFoundResult.Value as BaseErrorResponse;

            response.Should().NotBeNull();

            response.StatusCode.Should().Be(404);
            response.Message.Should().BeEquivalentTo("No Charge by provided Id cannot be found!");
            response.Details.Should().BeEquivalentTo("");
        }

        [Fact]
        public async Task GetByIdReturns500()
        {
            _getByIdUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Test exception"));

            try
            {
                var result = await _chargeController.Get(new Guid("b45d2bbf-abec-454c-a843-4667786177a1"), new Guid("b45d2bbf-abec-454c-a843-4667786177a1"))
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
        public async Task AddChargeWithValidDataReturns201()
        {
            _addUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<AddChargeRequest>(), It.IsAny<string>()))
                .ReturnsAsync(new ChargeResponse
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

            var charge = new AddChargeRequest
            {
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
            };


            var result = await _chargeController.Post(Guid.NewGuid().ToString(), Token, charge).ConfigureAwait(false);

            result.Should().NotBeNull();

            var createdAtActionResult = result as CreatedAtActionResult;

            createdAtActionResult.Should().NotBeNull();

            createdAtActionResult.ActionName.Should().BeEquivalentTo("Get");

            createdAtActionResult.RouteValues["id"].Should().NotBeNull();

            createdAtActionResult.RouteValues["id"].Should().BeOfType(typeof(Guid));

            createdAtActionResult.Value.Should().NotBeNull();

            var chargeResponse = createdAtActionResult.Value as ChargeResponse;

            chargeResponse.Should().NotBeNull();

            chargeResponse.Should().BeEquivalentTo(charge);
        }

        [Fact]
        public async Task AddChargeWithNullReturns400()
        {
            var result = await _chargeController.Post(null, null, null).ConfigureAwait(false);

            var badRequest = result as BadRequestObjectResult;

            badRequest.Should().NotBeNull();

            var error = badRequest.Value as BaseErrorResponse;

            error.Should().NotBeNull();

            error.StatusCode.Should().Be(400);

            error.Message.Should().BeEquivalentTo("Charge model cannot be null!");

            error.Details.Should().BeEquivalentTo("");
        }

        [Fact]
        public async Task PatchChargeWithValidModelReturns200()
        {
            var charge = new UpdateChargeRequest
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
            };

            _getByIdUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new ChargeResponse
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
                        Amount = 120,
                        Frequency = "Frequency"
                    }
                }
                });

            _updateUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<ChargeResponse>(), It.IsAny<string>()))
                .ReturnsAsync(new ChargeResponse
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
            var patchDoc = new JsonPatchDocument<UpdateChargeRequest>();
            patchDoc.Add(_ => _.ChargeGroup, ChargeGroup.Tenants);
            patchDoc.Add(_ => _.ChargeSubGroup, ChargeSubGroup.Actual);
            patchDoc.Add(_ => _.ChargeYear, 2022);
            patchDoc.Add(_ => _.TargetId, charge.TargetId);
            patchDoc.Add(_ => _.Id, charge.Id);
            patchDoc.Add(_ => _.DetailedCharges, charge.DetailedCharges);
            patchDoc.Add(_ => _.TargetType, charge.TargetType);


            var result = await _chargeController.Patch(Guid.NewGuid().ToString(), Token, new Guid("a3833a1d-0bd4-4cd2-a1cf-7db57b416505"),
                new Guid("59ca03ad-6c5c-49fa-8b7b-664e370417da"), patchDoc)
                .ConfigureAwait(false);

            var redirectToActionResult = result as OkObjectResult;

            redirectToActionResult.Should().NotBeNull();
        }

        [Fact]
        public async Task PutChargeWithNullReturns400()
        {
            var result = await _chargeController.Patch(string.Empty, string.Empty, Guid.NewGuid(), Guid.NewGuid(),
                null).ConfigureAwait(false);

            var badRequest = result as BadRequestObjectResult;

            badRequest.Should().NotBeNull();

            var error = badRequest.Value as BaseErrorResponse;

            error.Should().NotBeNull();

            error.StatusCode.Should().Be(400);

            error.Message.Should().BeEquivalentTo("Charge model cannot be null!");

            error.Details.Should().BeEquivalentTo("");
        }

        [Fact]
        public async Task PutChargeWithInvalidIdReturns404()
        {
            _getByIdUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((ChargeResponse) null);

            var guid = Guid.NewGuid();
            var patchDoc = new JsonPatchDocument<UpdateChargeRequest>();
            patchDoc.Add(_ => _.Id, guid);

            var result = await _chargeController.Patch(Guid.NewGuid().ToString(), Token, Guid.NewGuid(), Guid.NewGuid(), patchDoc)
                .ConfigureAwait(false);

            var notFoundResult = result as NotFoundObjectResult;

            notFoundResult.Should().NotBeNull();

            var error = notFoundResult.Value as BaseErrorResponse;

            error.Should().NotBeNull();

            error.StatusCode.Should().Be(404);

            error.Message.Should().BeEquivalentTo("No Charge by Id cannot be found!");

            error.Details.Should().BeEquivalentTo("");
        }

        [Fact]
        public async Task RemoveChargeWithValidIdReturns204()
        {
            _removeUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));

            _getByIdUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new ChargeResponse());

            var result = await _chargeController.Delete(Guid.NewGuid(), Guid.NewGuid()).ConfigureAwait(false);

            var noContent = result as NoContentResult;

            noContent.Should().NotBeNull();
        }

        [Fact]
        public async Task RemoveChargeWithInvalidIdReturns404()
        {
            _getByIdUseCase.Setup(_ => _.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((ChargeResponse) null);

            var result = await _chargeController.Delete(Guid.NewGuid(), Guid.NewGuid()).ConfigureAwait(false);

            var notFoundResult = result as NotFoundObjectResult;

            notFoundResult.Should().NotBeNull();

            var error = notFoundResult.Value as BaseErrorResponse;

            error.Should().NotBeNull();

            error.StatusCode.Should().Be(404);

            error.Message.Should().BeEquivalentTo("No Charge by Id cannot be found!");

            error.Details.Should().BeEquivalentTo("");
        }
    }
}
