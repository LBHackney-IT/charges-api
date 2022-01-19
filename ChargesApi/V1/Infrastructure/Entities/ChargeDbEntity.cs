using Amazon.DynamoDBv2.DataModel;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.Converters;
using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Infrastructure.Entities
{
    [DynamoDBTable("Charges", LowerCamelCaseProperties = true)]
    public class ChargeDbEntity
    {
        [DynamoDBHashKey(AttributeName = "target_id")]
        public Guid TargetId { get; set; }

        [DynamoDBRangeKey(AttributeName = "id")]
        public Guid Id { get; set; }

        [DynamoDBProperty(AttributeName = "target_type", Converter = typeof(DynamoDbEnumConverter<TargetType>))]
        public TargetType TargetType { get; set; }

        [DynamoDBProperty(AttributeName = "charge_group", Converter = typeof(DynamoDbEnumConverter<ChargeGroup>))]
        public ChargeGroup ChargeGroup { get; set; }

        [DynamoDBProperty(AttributeName = "detailed_charges", Converter = (typeof(DynamoDbObjectListConverter<DetailedCharges>)))]
        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }

        [DynamoDBProperty(AttributeName = "created_by")]
        public string CreatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_by")]
        public string LastUpdatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "created_at", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_at", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? LastUpdatedAt { get; set; }
    }
}
