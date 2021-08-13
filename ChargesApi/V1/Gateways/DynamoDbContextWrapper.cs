using Amazon.DynamoDBv2.DataModel;
using ChargeApi.V1.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargeApi.V1.Gateways
{
    public class DynamoDbContextWrapper
    {
        public virtual Task<List<ChargeDbEntity>> ScanAsync(IDynamoDBContext context, IEnumerable<ScanCondition> conditions, DynamoDBOperationConfig operationConfig = null)
        {
            return context.ScanAsync<ChargeDbEntity>(conditions, operationConfig).GetRemainingAsync();
        }
        public virtual Task<ChargesMaintenanceDbEntity> LoadAsync(IDynamoDBContext context, Guid id, DynamoDBOperationConfig operationConfig = null)
        {
            return context.LoadAsync<ChargesMaintenanceDbEntity>(id, operationConfig);
        }
    }
}
