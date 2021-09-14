using Amazon.DynamoDBv2.DataModel;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.Converters;
using System;

namespace ChargesApi.V1.Infrastructure.Entities
{
    [DynamoDBTable("ChargesList", LowerCamelCaseProperties = true)]
    public class ChargesListDbEntity
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty(AttributeName = "charge_name")]
        public string ChargeName { get; set; }

        [DynamoDBProperty(AttributeName = "charge_code")]
        public string ChargeCode { get; set; }

        [DynamoDBProperty(AttributeName = "charge_type", Converter = typeof(DynamoDbEnumConverter<ChargeType>))]
        public ChargeType ChargeType { get; set; }

        [DynamoDBProperty(AttributeName = "charge_group", Converter = typeof(DynamoDbEnumConverter<ChargeGroup>))]
        public ChargeGroup ChargeGroup { get; set; }

        [DynamoDBProperty(AttributeName = "created_by")]
        public string CreatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_by")]
        public string LastUpdatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "created_date", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime CreatedDate { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_date", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime LastUpdatedDate { get; set; }
    }
}
