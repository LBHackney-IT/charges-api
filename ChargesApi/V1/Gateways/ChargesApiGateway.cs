using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Infrastructure;
using ChargesApi.V1.Infrastructure.Entities;
using Hackney.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Gateways.Common;

namespace ChargesApi.V1.Gateways
{
    public class ChargesApiGateway : IChargesApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly IAmazonDynamoDB _amazonDynamoDb;

        public ChargesApiGateway(IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb ?? throw new ArgumentNullException(nameof(amazonDynamoDb));
        }

        public ChargesApiGateway(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB amazonDynamoDb)
        {
            _dynamoDbContext = dynamoDbContext ?? throw new ArgumentNullException(nameof(dynamoDbContext));
            _amazonDynamoDb = amazonDynamoDb ?? throw new ArgumentNullException(nameof(amazonDynamoDb));
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
            int totalSegments = 5;
            var finalResult = new List<Charge>();
            LoggingHandler.LogInfo($"*** Creating {totalSegments} Parallel Scan Tasks to scan {Constants.ChargeTableName}");
            Task[] tasks = new Task[totalSegments];
            for (int segment = 0; segment < totalSegments; segment++)
            {
                int tmpSegment = segment;
                Task task = await Task.Factory.StartNew(async () =>
                {
                    var scanSegmentResult = await ScanSegment(totalSegments, tmpSegment, queryParameters).ConfigureAwait(false);

                    finalResult.AddRange(scanSegmentResult);
                }).ConfigureAwait(false);

                tasks[segment] = task;
            }

            LoggingHandler.LogInfo("All scan tasks are created, waiting for them to complete.");
            Task.WaitAll(tasks);

            LoggingHandler.LogInfo("All scan tasks are completed.");
            LoggingHandler.LogInfo($"*** Completed Count:  {finalResult.Count} ");


            LoggingHandler.LogInfo("Scan completed");

            return finalResult;
        }

        private async Task<List<Charge>> ScanSegment(int totalSegments, int segment, PropertyChargesQueryParameters queryParameters)
        {
            var resultList = new List<Charge>();

            LoggingHandler.LogInfo($"*** Starting to Scan Segment {segment} of {Constants.ChargeTableName} out of {totalSegments} total segments ***");
            Dictionary<string, AttributeValue> lastEvaluatedKey = null;
            int totalScannedItemCount = 0;
            int totalScanRequestCount = 0;
            do
            {
                var request = new ScanRequest
                {
                    TableName = Constants.ChargeTableName,
                    Limit = 1200,
                    ExclusiveStartKey = lastEvaluatedKey,
                    Segment = segment,
                    TotalSegments = totalSegments
                };

                var response = await _amazonDynamoDb.ScanAsync(request).ConfigureAwait(false);
                lastEvaluatedKey = response.LastEvaluatedKey;
                totalScanRequestCount++;
                totalScannedItemCount += response.ScannedCount;

                var scannedResult = response.ToChargeDomain();
                LoggingHandler.LogInfo($"*** Completed Scan Count : {scannedResult.Count} ");
                var filteredList = scannedResult?.Where(x => x.ChargeYear == queryParameters.ChargeYear
                                                             && x.ChargeGroup == queryParameters.ChargeGroup
                                                             && x.ChargeSubGroup == queryParameters.ChargeSubGroup);

                resultList.AddRange(filteredList);
                LoggingHandler.LogInfo($"*** Completed Filtered Count:  {resultList.Count} ");
                Thread.Sleep(2000);
            } while (lastEvaluatedKey.Count != 0);

            LoggingHandler.LogInfo($"*** Completed Scan Segment {segment} of {Constants.ChargeTableName}. TotalScanRequestCount: {totalScanRequestCount}, TotalScannedItemCount: {totalScannedItemCount} ***");
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

        public async Task RemoveRangeAsync(List<ChargeKeys> keys)
        {
            var actions = new List<TransactWriteItem>();
            LoggingHandler.LogInfo($"RemoveRange started, keys count is:{keys.Count}");
            foreach (var key in keys)
            {
                actions.Add(new TransactWriteItem
                {
                    Delete = new Delete()
                    {
                        TableName = "Charges",
                        Key = new Dictionary<string, AttributeValue>
                        {
                            {"target_id",new AttributeValue(key.TargetId.ToString())},
                            {"id",new AttributeValue(key.Id.ToString())}
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
                var writeResult = await _amazonDynamoDb.TransactWriteItemsAsync(placeOrderCharge).ConfigureAwait(false);
                LoggingHandler.LogInfo("TransactWriteItemsAsync completed.");
                if (writeResult.HttpStatusCode != HttpStatusCode.OK)
                    throw new Exception(writeResult.ResponseMetadata.ToString());

            }
            catch (Exception ex)
            {
                LoggingHandler.LogInfo($"{keys}");
                LoggingHandler.LogInfo($"TransactWriteItemsAsync: {ex.Message}");
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
                LoggingHandler.LogWarning($"One of the table involved in the transaction is not found: {rnf.Message}");
            }
            catch (InternalServerErrorException ise)
            {
                LoggingHandler.LogWarning(($"Internal Server Error: {ise.Message}"));
            }
            catch (TransactionCanceledException tce)
            {
                LoggingHandler.LogWarning(($"Transaction Canceled: {tce.Message}"));
            }
            catch (Exception e)
            {
                LoggingHandler.LogWarning(($"Transaction Canceled: {e.Message}"));
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

            if (totalCount % batchCapacity == 0)
                loopCount = totalCount / batchCapacity;
            else
                loopCount = (totalCount / batchCapacity) + 1;

            for (var i = 0; i < loopCount; i++)
            {
                await DeleteBatchAsync(chargeKeysEnumerable.Skip(i * batchCapacity).Take(batchCapacity).ToList())
                    .ConfigureAwait(false);
            }
        }

        private async Task DeleteBatchAsync(List<ChargeKeys> chargeIds)
        {
            var actions = new List<TransactWriteItem>();
            LoggingHandler.LogInfo($"Items to delete {chargeIds.Count}");
            foreach (var charge in chargeIds.ToList())
            {
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
                var writeResult = await _amazonDynamoDb.TransactWriteItemsAsync(placeOrderCharge).ConfigureAwait(false);
                LoggingHandler.LogInfo("TransactWriteItemsAsync completed.");
                if (writeResult.HttpStatusCode != HttpStatusCode.OK)
                    throw new Exception(writeResult.ResponseMetadata.ToString());

            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"TransactWriteItemsAsync: {ex.Message}");
                throw;
            }
        }
    }
}
