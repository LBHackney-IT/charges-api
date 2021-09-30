using Amazon.DynamoDBv2.DataModel;
using ChargesApi.V1.Infrastructure.Converters;
using System;

namespace ChargesApi.V1.Infrastructure.Entities
{
    [DynamoDBTable("Estimates", LowerCamelCaseProperties = true)]
    public class EstimatesDbEntity
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty(AttributeName = "leaseholder_name")]
        public string LeaseholderName { get; set; }

        [DynamoDBProperty(AttributeName = "prn")]
        public string Prn { get; set; }

        [DynamoDBProperty(AttributeName = "block_name")]
        public string BlockName { get; set; }

        [DynamoDBProperty(AttributeName = "estate_name")]
        public string EstateName { get; set; }

        [DynamoDBProperty(AttributeName = "monthly_amount")]
        public decimal MonthlyAmount { get; set; }

        [DynamoDBProperty(AttributeName = "yearly_amount")]
        public decimal YearlyAmount { get; set; }

        [DynamoDBProperty(AttributeName = "estimate_year")]
        public short EstimateYear { get; set; }

        [DynamoDBProperty(AttributeName = "created_by")]
        public string CreatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_by")]
        public string LastUpdatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "created_at", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_at", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime LastUpdatedAt { get; set; }
    }
}
