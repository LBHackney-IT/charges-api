using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase;
using ChargesApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ChargesApi
{
    public class LambdaHandler
    {
        private readonly IGetAllUseCase _getAllUseCase;
        private readonly IRemoveRangeUseCase _removeRangeUseCase;

        public LambdaHandler()
        {
            IAmazonDynamoDB amazonDynamoDb = CreateAmazonDynamoDbClient();
            IChargesApiGateway apiGateway = new ChargesApiGateway(amazonDynamoDb);
            _getAllUseCase = new GetAllUseCase(apiGateway);
            _removeRangeUseCase = new RemoveRangeUseCase(apiGateway);
        }

        public async Task<StepResponse> DeleteRange([FromBody] List<Guid> targetIds)
        {
            if (targetIds == null) throw new ArgumentNullException(nameof(targetIds));
            if (targetIds.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(targetIds));

            List<ChargeKeys> keysList = new List<ChargeKeys>();
            foreach (var targetId in targetIds)
            {
                var charges = await _getAllUseCase.ExecuteAsync(targetId).ConfigureAwait(false);
                charges.ForEach(c =>
                {
                    keysList.Add(new ChargeKeys(c.Id, c.TargetId));
                });
            }

            for (int i = 0; i <= keysList.Count / 25; i++)
                await _removeRangeUseCase.ExecuteAsync(keysList.Skip(i * 25).Take(25).ToList()).ConfigureAwait(false);

            return new StepResponse()
            {
                Continue = false,
                NextStepTime = DateTime.Now.AddSeconds(15)
            };
        }

        private static AmazonDynamoDBClient CreateAmazonDynamoDbClient()
        {
            bool result = bool.Parse(value: Environment.GetEnvironmentVariable("DynamoDb_LocalMode") ?? "false");
            if (result)
            {
                string url = Environment.GetEnvironmentVariable("DynamoDb_LocalServiceUrl");
                return new AmazonDynamoDBClient(new AmazonDynamoDBConfig
                {
                    ServiceURL = url
                });
            }
            return new AmazonDynamoDBClient();
        }

    }
}
