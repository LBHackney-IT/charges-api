using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.Entities;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.E2ETests
{
    public class DynamoDbChargeMaintenanceIntegrationTests : DynamoDbIntegrationTests<Startup>
    {
        private static AddChargeMaintenanceRequest ConstructChargeMaintenance()
        {
            var entity = new AddChargeMaintenanceRequest
            {
                ChargesId = Guid.NewGuid(),
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
                StartDate = new DateTime(2021, 07, 10, 00, 00, 0, DateTimeKind.Utc),
                Status = ChargeMaintenanceStatus.Pending
            };

            return entity;
        }
        [Fact]
        public async Task GetChargeMaintenanceByIdNotFoundReturns404()
        {
            Guid id = Guid.NewGuid();

            var uri = new Uri($"api/v1/charges-maintenance/{id}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<BaseErrorResponse>(responseContent);

            apiEntity.Should().NotBeNull();
            apiEntity.Message.Should().BeEquivalentTo("No Charge Maintenance by provided Id cannot be found!");
            apiEntity.StatusCode.Should().Be(404);
            apiEntity.Details.Should().BeEquivalentTo(string.Empty);
        }

        //[Fact]
        //public async Task CreateChargeMaintenanceAndThenGetByIdReturns201()
        //{
        //    var charge = DynamoDbChargeIntegrationTests.ConstructCharge();
        //    var chargeResponse = await CreateChargeAndValidateResponse(charge).ConfigureAwait(false);

        //    var chargeMaintenance = ConstructChargeMaintenance();
        //    chargeMaintenance.ChargesId = chargeResponse.Id;
        //    chargeMaintenance.TargetId = chargeResponse.TargetId;

        //    var response = await CreateChargeMaintenanceAndValidateResponse(chargeMaintenance).ConfigureAwait(false);

        //    await GetChargeMaintenanceByIdAndValidateResponse(response.Id, response).ConfigureAwait(false);
        //}

        [Fact]
        public async Task CreateChargeMaintenanceBadRequestReturns400()
        {
            var chargeMaintenance = new AddChargeMaintenanceRequest();

            var uri = new Uri("api/v1/charges-maintenance", UriKind.Relative);
            string body = JsonConvert.SerializeObject(chargeMaintenance);

            HttpResponseMessage response;
            using var stringContent = new StringContent(body);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            response = await Client.PostAsync(uri, stringContent).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<BaseErrorResponse>(responseContent);

            apiEntity.Should().NotBeNull();
            apiEntity.StatusCode.Should().Be(400);
            apiEntity.Details.Should().BeEquivalentTo("");
        }
        private async Task<ChargeMaintenanceResponse> CreateChargeMaintenanceAndValidateResponse(AddChargeMaintenanceRequest chargeMaintenance)
        {
            var uri = new Uri($"api/v1/charges-maintenance", UriKind.Relative);

            string body = JsonConvert.SerializeObject(chargeMaintenance);

            using StringContent stringContent = new StringContent(body);

            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var response = await Client.PostAsync(uri, stringContent).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ChargeMaintenanceResponse>(responseContent);

            CleanupActions.Add(async () => await DynamoDbContext.DeleteAsync<ChargesMaintenanceDbEntity>(apiEntity.Id).ConfigureAwait(false));

            apiEntity.Should().NotBeNull();

            chargeMaintenance.Should().BeEquivalentTo(apiEntity, options => options.Excluding(a => a.Id));

            return apiEntity;
        }
        private async Task<ChargeResponse> CreateChargeAndValidateResponse(Charge charge)
        {
            var uri = new Uri($"api/v1/charges", UriKind.Relative);

            string body = JsonConvert.SerializeObject(charge);

            using StringContent stringContent = new StringContent(body);

            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var response = await Client.PostAsync(uri, stringContent).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ChargeResponse>(responseContent);

            CleanupActions.Add(async () => await DynamoDbContext.DeleteAsync<ChargeDbEntity>(apiEntity.TargetId, apiEntity.Id).ConfigureAwait(false));

            apiEntity.Should().NotBeNull();

            apiEntity.Should().BeEquivalentTo(charge, options => options.Excluding(a => a.Id));

            return apiEntity;
        }

        private async Task GetChargeMaintenanceByIdAndValidateResponse(Guid id, ChargeMaintenanceResponse chargeMaintenance = null)
        {
            var uri = new Uri($"api/v1/charges-maintenance/{id}", UriKind.Relative);
            using var response = await Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ChargeMaintenanceResponse>(responseContent);

            chargeMaintenance.Should().BeEquivalentTo(apiEntity, opt => opt.Excluding(x => x.Id));
        }
    }
}
