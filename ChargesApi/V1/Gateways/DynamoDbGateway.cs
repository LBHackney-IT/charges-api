using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Infrastructure;
using ChargesApi.V1.Infrastructure.Entities;
using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;
using Microsoft.Extensions.Configuration;
using ChargesApi.V1.Infrastructure.JWT;
using ChargesApi.V1.Gateways.Common;

namespace ChargesApi.V1.Gateways
{
    public class DynamoDbGateway : IChargesApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly ILogger<IChargesApiGateway> _logger;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext,
            IAmazonDynamoDB amazonDynamoDb,
            ILogger<IChargesApiGateway> logger)
        {
            _dynamoDbContext = dynamoDbContext;
            _amazonDynamoDb = amazonDynamoDb;
            _logger = logger;
        }

        public void Add(Charge charge)
        {
            _dynamoDbContext.SaveAsync(charge.ToDatabase());
        }

        public async Task AddAsync(Charge charge)
        {
            await _dynamoDbContext.SaveAsync(charge.ToDatabase()).ConfigureAwait(false);
        }

        public void AddRange(List<Charge> charges)
        {
            charges.ForEach(e =>
            {
                _dynamoDbContext.SaveAsync(e.ToDatabase());
            });
        }

        public async Task AddRangeAsync(List<Charge> charges)
        {
            foreach (Charge charge in charges)
            {
                await _dynamoDbContext.SaveAsync(charge.ToDatabase()).ConfigureAwait(false);
            }
        }

        public async Task<List<Charge>> GetAllChargesAsync(Guid targetId)
        {
            var request = new QueryRequest
            {
                TableName = Constants.ChargeTableName,
                KeyConditionExpression = "target_id = :V_target_id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":V_target_id",new AttributeValue{S = targetId.ToString()}}
                },
                ScanIndexForward = true
            };

            var chargesLists = await _amazonDynamoDb.QueryAsync(request).ConfigureAwait(false);

            return chargesLists?.ToChargeDomain();
        }

        public async Task<IList<Charge>> GetChargesAsync(PropertyChargesQueryParameters queryParameters)
        {
            var resultList = new List<Charge>();
            var scanResult = new List<Charge>();



            Dictionary<string, AttributeValue> lastEvaluatedKey = null;
            do
            {
                var scanRequest = new ScanRequest
                {
                    TableName = Constants.ChargeTableName,
                    //FilterExpression = "charge_year = :v_charge_year and charge_group = :v_charge_group and charge_sub_group = :v_charge_sub_group",
                    //ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    //{
                    //    { ":v_charge_year", new AttributeValue { N = queryParameters.ChargeYear.ToString() } },
                    //    { ":v_charge_group", new AttributeValue { S = queryParameters.ChargeGroup.ToString() } },
                    //    { ":v_charge_sub_group", new AttributeValue { S = queryParameters.ChargeSubGroup.ToString() } },
                    //},
                    ExclusiveStartKey = lastEvaluatedKey
                };
                var chargesLists = await _amazonDynamoDb.ScanAsync(scanRequest).ConfigureAwait(false);
                lastEvaluatedKey = chargesLists.LastEvaluatedKey;

                scanResult = chargesLists?.ToChargeDomain();
                var filteredList = scanResult?.Where(x => x.ChargeYear == queryParameters.ChargeYear
                                                          && x.ChargeGroup == queryParameters.ChargeGroup
                                                          && x.ChargeSubGroup == queryParameters.ChargeSubGroup);

                LoggingHandler.LogInfo($"Items Count {filteredList.Count()}");
                if (filteredList != null && filteredList.Any())
                    resultList.AddRange(filteredList);
            } while (scanResult != null && scanResult.Any());

            LoggingHandler.LogInfo("Scan completed");
            LoggingHandler.LogInfo($"Total Items Count {resultList.Count}");
            //ScanRequest request = new ScanRequest(Constants.ChargeTableName)
            //{
            //    Limit = 940,
            //    FilterExpression = "charge_year = :v_charge_year and charge_group = :v_charge_group and charge_sub_group = :v_charge_sub_group",
            //    ExclusiveStartKey = lastEvaluatedKey
            //};



            return resultList;
        }

        public async Task<Charge> GetChargeByIdAsync(Guid id, Guid targetId)
        {
            var result = await _dynamoDbContext.LoadAsync<ChargeDbEntity>(targetId, id).ConfigureAwait(false);

            return result?.ToDomain();
        }

        public void Remove(Charge charge)
        {
            _dynamoDbContext.DeleteAsync(charge.ToDatabase());
        }

        public async Task RemoveAsync(Charge charge)
        {
            await _dynamoDbContext.DeleteAsync(charge.ToDatabase()).ConfigureAwait(false);
        }

        public void RemoveRange(List<Charge> charges)
        {
            charges.ForEach(c =>
            {
                Remove(c);
            });
        }

        public async Task RemoveRangeAsync(List<Charge> charges)
        {
            foreach (Charge c in charges)
            {
                await RemoveAsync(c).ConfigureAwait(false);
            }
        }

        public void Update(Charge charge)
        {
            _dynamoDbContext.SaveAsync(charge.ToDatabase());
        }

        public async Task UpdateAsync(Charge charge)
        {
            await _dynamoDbContext.SaveAsync(charge.ToDatabase()).ConfigureAwait(false);
        }

        public async Task<bool> AddBatchAsync(List<Charge> charges)
        {
            var chargesBatch = _dynamoDbContext.CreateBatchWrite<ChargeDbEntity>();

            var items = charges.ToDatabaseList();
            var maxBatchCount = Constants.PerBatchProcessingCount;
            if (items.Count > maxBatchCount)
            {
                var loopCount = (items.Count / maxBatchCount) + 1;
                for (var start = 0; start < loopCount; start++)
                {
                    var itemsToWrite = items.Skip(start * maxBatchCount).Take(maxBatchCount);
                    chargesBatch.AddPutItems(itemsToWrite);
                    await chargesBatch.ExecuteAsync().ConfigureAwait(false);
                    Thread.Sleep(1000);
                }
            }
            else
            {
                chargesBatch.AddPutItems(items);
                await chargesBatch.ExecuteAsync().ConfigureAwait(false);
            }

            return true;
        }
        [LogCall]
        public async Task<bool> AddTransactionBatchAsync(List<Charge> charges)
        {
            bool result = false;

            List<TransactWriteItem> actions = new List<TransactWriteItem>();
            foreach (Charge charge in charges)
            {
                Dictionary<string, AttributeValue> columns = new Dictionary<string, AttributeValue>();
                columns = charge.ToQueryRequest();

                actions.Add(new TransactWriteItem
                {
                    Put = new Put()
                    {
                        TableName = Constants.ChargeTableName,
                        Item = columns,
                        ReturnValuesOnConditionCheckFailure = ReturnValuesOnConditionCheckFailure.ALL_OLD,
                        ConditionExpression = Constants.AttributeNotExistId
                    }
                });
            }

            TransactWriteItemsRequest placeOrderCharges = new TransactWriteItemsRequest
            {
                TransactItems = actions,
                ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL
            };

            try
            {
                await _amazonDynamoDb.TransactWriteItemsAsync(placeOrderCharges).ConfigureAwait(false);
                result = true;
            }
            catch (ResourceNotFoundException rnf)
            {
                _logger.LogDebug($"One of the table involved in the transaction is not found: {rnf.Message}");
            }
            catch (InternalServerErrorException ise)
            {
                _logger.LogDebug($"Internal Server Error: {ise.Message}");
            }
            catch (TransactionCanceledException tce)
            {
                _logger.LogDebug($"Transaction Canceled: {tce.Message}");
            }
            catch (Exception e)
            {
                _logger.LogDebug($"Transaction Canceled: {e.Message}");
                throw new Exception(e.Message);
            }
            return result;
        }

        public async Task DeleteBatchAsync(IEnumerable<ChargeKeys> chargeIds, int batchCapacity)
        {
            if (batchCapacity <= 0)
            {
                return;
            }

            int loopCount;
            var chargeKeysEnumerable = chargeIds.ToList();
            var totalCount = chargeKeysEnumerable.ToList().Count;

            LoggingHandler.LogInfo($"Items to delete {totalCount}");
            //_logger.LogInformation($"Items to delete {totalCount}");

            if (totalCount % batchCapacity == 0)
                loopCount = totalCount / batchCapacity;
            else
                loopCount = (totalCount / batchCapacity) + 1;

            for (var i = 0; i < loopCount; i++)
            {
                await DeleteBatchAsync(chargeKeysEnumerable.Skip(i * batchCapacity).Take(batchCapacity))
                    .ConfigureAwait(false);
            }
        }

        private async Task DeleteBatchAsync(IEnumerable<ChargeKeys> chargeIds)
        {
            var actions = new List<TransactWriteItem>();
            _logger.LogInformation($"Items to delete {chargeIds.Count()}");
            foreach (var charge in chargeIds)
            {
                //var columns = transaction.ToQueryRequest();

                actions.Add(new TransactWriteItem
                {
                    Delete = new Delete()
                    {
                        TableName = "Charges",
                        Key = new Dictionary<string, AttributeValue>
                        {
                            {"target_id",new AttributeValue(charge.TargetId.ToString())},
                            {"id",new AttributeValue(charge.Id.ToString())}
                        },
                        ReturnValuesOnConditionCheckFailure = ReturnValuesOnConditionCheckFailure.ALL_OLD
                    },
                });
            }

            TransactWriteItemsRequest placeOrderCharge = new TransactWriteItemsRequest
            {
                TransactItems = actions,
                ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL
            };
            try
            {
                LoggingHandler.LogInfo("TransactWriteItemsAsync starting.");
                // _logger.LogInformation($"TransactWriteItemsAsync starting");
                var writeResult = await _amazonDynamoDb.TransactWriteItemsAsync(placeOrderCharge).ConfigureAwait(false);
                LoggingHandler.LogInfo("TransactWriteItemsAsync completed.");
                //_logger.LogInformation($"TransactWriteItemsAsync completed");
                if (writeResult.HttpStatusCode != HttpStatusCode.OK)
                    throw new Exception(writeResult.ResponseMetadata.ToString());

            }
            catch (Exception ex)
            {
                _logger.LogError($"TransactWriteItemsAsync: {ex.Message}");
                throw;
            }

            //var request = new BatchWriteItemRequest
            //{
            //    ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL,
            //    RequestItems = new Dictionary<string, List<WriteRequest>>
            //    {
            //        {
            //            Constants.ChargeTableName,
            //            chargeIds.ToWriteRequests().ToList()
            //        }
            //    }
            //};

            //BatchWriteItemResponse response;
            //do
            //{
            //    response = await _amazonDynamoDb.BatchWriteItemAsync(request).ConfigureAwait(false);

            //    request.RequestItems = response.UnprocessedItems;
            //}
            //while (response.UnprocessedItems.Count > 0);


        }

        public async Task<IEnumerable<ChargeKeys>> ScanByYearGroupSubGroup(short chargeYear, ChargeGroup chargeGroup, ChargeSubGroup? chargeSubGroup)
        {
            var scanRequest = new ScanRequest
            {
                TableName = Constants.ChargeTableName,
                FilterExpression = "charge_year = :v_charge_year and charge_group = :v_charge_group",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":v_charge_year", new AttributeValue { N = chargeYear.ToString() } },
                    { ":v_charge_group", new AttributeValue { S = chargeGroup.ToString() } }
                }
            };

            if (chargeSubGroup != null)
            {
                scanRequest.FilterExpression += " and charge_sub_group = :v_charge_sub_group";
                scanRequest.ExpressionAttributeValues.Add(":v_charge_sub_group", new AttributeValue { S = chargeSubGroup.Value.ToString() });
            }
            else
            {
                scanRequest.FilterExpression += " and attribute_not_exists(charge_sub_group)";
            }

            var response = await _amazonDynamoDb.ScanAsync(scanRequest).ConfigureAwait(false);
            LoggingHandler.LogInfo($"Total Count: {response.Count}.");
            return response.Items.Select(i => i.GetChargeKeys());


        }
    }
}
