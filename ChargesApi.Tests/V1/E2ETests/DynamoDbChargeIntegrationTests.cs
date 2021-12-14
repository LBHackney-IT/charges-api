using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ChargesApi.V1.Boundary;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure;
using ChargesApi.V1.Infrastructure.Entities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.E2ETests
{
    public class DynamoDbChargeIntegrationTests : DynamoDbIntegrationTests<Startup>
    {
        public static Charge ConstructCharge()
        {
            var entity = new Charge
            {
                Id = Guid.NewGuid(),
                TargetId = Guid.NewGuid(),
                TargetType = TargetType.Asset,
                DetailedCharges = new List<DetailedCharges>()
            };

            return entity;
        }

        [Fact]
        public async Task GetChargeByIdNotFoundReturns404()
        {
            Guid id = Guid.NewGuid();

            var uri = new Uri($"api/v1/charges/{id}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<BaseErrorResponse>(responseContent);

            apiEntity.Should().NotBeNull();
            apiEntity.Message.Should().BeEquivalentTo("No Charge by provided Id cannot be found!");
            apiEntity.StatusCode.Should().Be(404);
            apiEntity.Details.Should().BeEquivalentTo(string.Empty);
        }

        [Fact]
        public async Task HealthCheckOkReturns200()
        {
            var uri = new Uri($"api/v1/healthcheck/ping", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<HealthCheckResponse>(responseContent);

            apiEntity.Should().NotBeNull();
            apiEntity.Message.Should().BeNull();
            apiEntity.Success.Should().BeTrue();
        }

        [Fact]
        public async Task CreateChargeAndThenGetByIdReturns201()
        {
            var charge = ConstructCharge();

            var response = await CreateChargeAndValidateResponse(charge).ConfigureAwait(false);

            await GetChargeByIdAndValidateResponse(response.Id, response).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateChargeBadRequestReturns400()
        {
            var charge = new Charge();

            var uri = new Uri("api/v1/charges", UriKind.Relative);
            string body = JsonConvert.SerializeObject(charge);

            HttpResponseMessage response;
            using var stringContent = new StringContent(body);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            response = await Client.PostAsync(uri, stringContent).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<BaseErrorResponse>(responseContent);

            apiEntity.Should().NotBeNull();
            apiEntity.StatusCode.Should().Be(400);
            apiEntity.Message.Should().BeEquivalentTo("Guid cannot be empty or default.");
            apiEntity.Details.Should().BeEquivalentTo("");
        }

        [Fact]
        public async Task CreateAndUpdateChargeReturns200()
        {
            var charge = ConstructCharge();

            var uri = new Uri($"api/v1/charges", UriKind.Relative);

            string body = JsonConvert.SerializeObject(charge);

            using StringContent stringContent = new StringContent(body);

            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var response = await Client.PostAsync(uri, stringContent).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ChargeResponse>(responseContent);

            apiEntity.Should().NotBeNull();

            apiEntity.Should().BeEquivalentTo(charge, options => options.Excluding(a => a.Id));

            CleanupActions.Add(() => DynamoDbContext.DeleteAsync<ChargeDbEntity>(apiEntity.TargetId, apiEntity.Id).ConfigureAwait(false));

            apiEntity.TargetType = TargetType.Block;

            var updateUri = new Uri($"api/v1/charges/{apiEntity.Id}?targetId={apiEntity.TargetId}", UriKind.Relative);
            string updateCharge = JsonConvert.SerializeObject(apiEntity);

            using var updateStringContent = new StringContent(updateCharge);
            updateStringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var updateResponse = await Client.PutAsync(updateUri, updateStringContent).ConfigureAwait(false);

            updateResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var updateResponseContent = await updateResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var updateApiEntity = JsonConvert.DeserializeObject<ChargeResponse>(updateResponseContent);

            updateApiEntity.Should().NotBeNull();
            updateApiEntity.TargetType.Should().Be(TargetType.Block);
        }

        [Fact]
        public async Task CreateAndDeleteChargeReturns204()
        {
            var charge = ConstructCharge();

            var uri = new Uri($"api/v1/charges", UriKind.Relative);

            string body = JsonConvert.SerializeObject(charge);

            using StringContent stringContent = new StringContent(body);

            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var response = await Client.PostAsync(uri, stringContent).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ChargeResponse>(responseContent);

            apiEntity.Should().NotBeNull();

            apiEntity.Should().BeEquivalentTo(charge, options => options.Excluding(a => a.Id));

            var deleteUri = new Uri($"api/v1/charges/{apiEntity.Id}?targetId={apiEntity.TargetId}", UriKind.Relative);

            using var deleteResponse = await Client.DeleteAsync(deleteUri).ConfigureAwait(false);

            deleteResponse.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task UpdateAndDeleteNotExistChargeReturns404()
        {
            var charge = ConstructCharge();
            string body = JsonConvert.SerializeObject(charge);

            var updateUri = new Uri($"api/v1/charges/{charge.Id}", UriKind.Relative);
            string updateCharge = JsonConvert.SerializeObject(charge);

            HttpResponseMessage updateResponse;
            using var updateStringContent = new StringContent(body);
            updateStringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            updateResponse = await Client.PutAsync(updateUri, updateStringContent).ConfigureAwait(false);

            updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var updateResponseContent = await updateResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var updateBaseErrorResponse = JsonConvert.DeserializeObject<BaseErrorResponse>(updateResponseContent);

            updateBaseErrorResponse.StatusCode.Should().Be(404);
            updateBaseErrorResponse.Message.Should().BeEquivalentTo("No Charge by Id cannot be found!");
            updateBaseErrorResponse.Details.Should().BeEquivalentTo("");

            var deleteUri = new Uri($"api/v1/charges/{charge.Id}", UriKind.Relative);
            using var deleteResponse = await Client.DeleteAsync(deleteUri).ConfigureAwait(false);

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var deleteResponseContent = await deleteResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var deleteBaseErrorResponse = JsonConvert.DeserializeObject<BaseErrorResponse>(deleteResponseContent);

            deleteBaseErrorResponse.StatusCode.Should().Be(404);
            deleteBaseErrorResponse.Message.Should().BeEquivalentTo("No Charge by Id cannot be found!");
            deleteBaseErrorResponse.Details.Should().BeEquivalentTo("");
        }


        // Hanna Holosova
        // Xunit support parallelism, which can run diffirent test classes at the same time.
        // This test set local mode to null and it poses problems in ConfigureDynamoDB,
        // so last test in E2E tried to connect to real database, not local
        // https://xunit.net/docs/running-tests-in-parallel

        [Theory]
        [InlineData(null)]
        [InlineData("false")]
        [InlineData("true")]
        public void ConfigureDynamoDBTestNoLocalModeEnvVarUsesAWSService(string localModeEnvVar)
        {
            Environment.SetEnvironmentVariable("DynamoDb_LocalMode", localModeEnvVar);

            ServiceCollection services = new ServiceCollection();
            services.ConfigureDynamoDB();

            services.Any(x => x.ServiceType == typeof(IAmazonDynamoDB)).Should().BeTrue();
            services.Any(x => x.ServiceType == typeof(IDynamoDBContext)).Should().BeTrue();

            Environment.SetEnvironmentVariable("DynamoDb_LocalMode", null);
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

        private async Task GetChargeByIdAndValidateResponse(Guid id, ChargeResponse charge = null)
        {
            var uri = new Uri($"api/v1/charges/{id}?targetId={charge.TargetId}", UriKind.Relative);
            using var response = await Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ChargeResponse>(responseContent);

            apiEntity.Should().BeEquivalentTo(charge);
        }
    }
}
