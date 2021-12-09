using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Infrastructure.Entities;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.E2ETests
{
    public class DynamoDbChargesListIntegrationTests : DynamoDbIntegrationTests<Startup>
    {
        private static AddChargesListRequest ConstructChargesList()
        {
            var entity = new AddChargesListRequest
            {
                ChargeCode = "LCT",
                ChargeGroup = ChargesApi.V1.Domain.ChargeGroup.Tenants,
                ChargeName = "Lighting",
                ChargeType = ChargesApi.V1.Domain.ChargeType.Block
            };
            return entity;
        }
        [Fact]
        public async Task GetChargesListByIdNotFoundReturns404()
        {
            Guid id = Guid.NewGuid();

            var uri = new Uri($"api/v1/charges-list/{id}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<BaseErrorResponse>(responseContent);

            apiEntity.Should().NotBeNull();
            apiEntity.Message.Should().BeEquivalentTo("No ChargesList by provided Id cannot be found!");
            apiEntity.StatusCode.Should().Be(404);
            apiEntity.Details.Should().BeEquivalentTo(string.Empty);
        }

        [Fact]
        public async Task CreateChargesListAndThenGetByIdReturns201()
        {
            var chargeListRequest = ConstructChargesList();
            var chargeResponse = await CreateChargeAndValidateResponse(chargeListRequest).ConfigureAwait(false);

            await GetChargesListByIdAndValidateResponse(chargeResponse.Id, chargeResponse).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateChargesListBadRequestReturns400()
        {
            var chargeMaintenance = new AddChargeMaintenanceRequest();

            var uri = new Uri("api/v1/charges-list", UriKind.Relative);
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
        private async Task<ChargesListResponse> CreateChargeAndValidateResponse(AddChargesListRequest chargesList)
        {
            var uri = new Uri($"api/v1/charges-list", UriKind.Relative);

            string body = JsonConvert.SerializeObject(chargesList);

            using StringContent stringContent = new StringContent(body);

            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var response = await Client.PostAsync(uri, stringContent).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ChargesListResponse>(responseContent);

            CleanupActions.Add(async () => await DynamoDbContext.DeleteAsync<ChargesListDbEntity>(apiEntity.Id).ConfigureAwait(false));

            apiEntity.Should().NotBeNull();

            chargesList.Should().BeEquivalentTo(apiEntity, options => options.Excluding(a => a.Id));

            return apiEntity;
        }

        private async Task GetChargesListByIdAndValidateResponse(Guid id, ChargesListResponse chargesList = null)
        {
            var uri = new Uri($"api/v1/charges-list/{id}", UriKind.Relative);
            using var response = await Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ChargesListResponse>(responseContent);

            chargesList.Should().BeEquivalentTo(apiEntity, opt => opt.Excluding(x => x.Id));
        }
    }
}
