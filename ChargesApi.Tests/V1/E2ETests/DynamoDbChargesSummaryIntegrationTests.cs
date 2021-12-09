using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.Entities;
using FluentAssertions;
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
    public class DynamoDbChargesSummaryIntegrationTests : DynamoDbIntegrationTests<Startup>
    {
        public static Charge ConstructCharge(Guid targetId, TargetType val)
        {
            var entity = new Charge
            {
                Id = Guid.NewGuid(),
                TargetId = targetId,
                TargetType = val,
                ChargeGroup = ChargeGroup.Tenants,
                DetailedCharges = new List<DetailedCharges>
                {
                    new DetailedCharges
                    {
                         Type = "service",
                         SubType = "Test Estate",
                         StartDate = new DateTime(2021, 7, 2),
                         EndDate = new DateTime(2022, 7, 4),
                         Amount = 150,
                         Frequency = "weekly",
                         ChargeCode = "TST",
                         ChargeType = ChargeType.Estate
                    },
                     new DetailedCharges
                    {
                         Type = "service",
                         SubType = "Test Block",
                         StartDate = new DateTime(2021, 7, 2),
                         EndDate = new DateTime(2022, 3, 4),
                         Amount = 150,
                         Frequency = "weekly",
                         ChargeCode = "TST",
                         ChargeType = ChargeType.Block
                    }
                }
            };
            return entity;
        }

        [Fact]
        public async Task GetEstateChargesSummaryByIdAndValidateResponse()
        {
            Guid id = Guid.NewGuid();
            var targetType = TargetType.Estate.ToString();
            var chargeRequest = ConstructCharge(id, TargetType.Estate);
            var chargeresponse = await CreateChargeAndValidateResponse(chargeRequest).ConfigureAwait(false);

            var uri = new Uri($"api/v1/charges-summary?targetId={id}&targetType={targetType}", UriKind.Relative);
            using var response = await Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ChargesSummaryResponse>(responseContent);

            apiEntity.TargetId.Should().Be(chargeresponse.TargetId);
            apiEntity.TargetType.Should().Be(TargetType.Estate);
            apiEntity.ChargesList.Should().NotBeNullOrEmpty();
            apiEntity.ChargesList.Should().HaveCount(2);

        }
        [Fact]
        public async Task GetBlockChargesSummaryByIdAndValidateResponse()
        {
            Guid id = Guid.NewGuid();
            var targetType = TargetType.Block.ToString();
            var chargeRequest = ConstructCharge(id, TargetType.Block);
            var chargeresponse = await CreateChargeAndValidateResponse(chargeRequest).ConfigureAwait(false);

            var uri = new Uri($"api/v1/charges-summary?targetId={id}&targetType={targetType}", UriKind.Relative);
            using var response = await Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ChargesSummaryResponse>(responseContent);

            apiEntity.TargetId.Should().Be(chargeresponse.TargetId);
            apiEntity.TargetType.Should().Be(TargetType.Block);
            apiEntity.ChargesList.Should().NotBeNullOrEmpty();
            apiEntity.ChargesList.Should().HaveCount(2);

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

            CleanupActions.Add(async () => await DynamoDbContext.DeleteAsync<ChargeDbEntity>(apiEntity.Id).ConfigureAwait(false));

            apiEntity.Should().NotBeNull();

            apiEntity.Should().BeEquivalentTo(charge, options => options.Excluding(a => a.Id));

            return apiEntity;
        }
    }
}
