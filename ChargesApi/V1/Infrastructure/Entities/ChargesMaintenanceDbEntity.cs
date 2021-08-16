using Amazon.DynamoDBv2.DataModel;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.Converters;
using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Infrastructure.Entities
{
    [DynamoDBTable("charges_maintenance", LowerCamelCaseProperties = true)]
    public class ChargesMaintenanceDbEntity
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty(AttributeName = "charges_id")]
        public Guid ChargesId { get; set; }


        [DynamoDBProperty(AttributeName = "existing_value", Converter = (typeof(DynamoDbObjectListConverter<DetailedCharges>)))]
        public IEnumerable<DetailedCharges> ExistingValue { get; set; }

        [DynamoDBProperty(AttributeName = "new_value", Converter = (typeof(DynamoDbObjectListConverter<DetailedCharges>)))]
        public IEnumerable<DetailedCharges> NewValue { get; set; }

        [DynamoDBProperty(AttributeName = "reason")]
        public string Reason { get; set; }

        [DynamoDBProperty(AttributeName = "start_date", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime StartDate { get; set; }

        [DynamoDBProperty(AttributeName = "status", Converter = typeof(DynamoDbEnumConverter<ChargeMaintenanceStatus>))]
        public ChargeMaintenanceStatus Status { get; set; }

        [DynamoDBProperty(AttributeName = "created_by")]
        public string CreatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "created_date", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime CreatedDate { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_by")]
        public string LastUpdatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_date", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime LastUpdatedDate { get; set; }
    }
}
