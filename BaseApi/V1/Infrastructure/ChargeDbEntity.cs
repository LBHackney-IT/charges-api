using Amazon.DynamoDBv2.DataModel;
using BaseApi.V1.Domain;
using ChargeApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApi.V1.Infrastructure
{ 
    [DynamoDBTable("charges",LowerCamelCaseProperties = true)]
    public class ChargeDbEntity
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty(AttributeName = "target_id")]
        public Guid TargetId { get; set; }

        [DynamoDBProperty(AttributeName = "target_type",Converter = typeof(DynamoDbEnumConverter<TargetType>))]
        public TargetType TargetType { get; set; }

        [DynamoDBProperty(AttributeName = "charge_type")] //,Converter = typeof(DynamoDbObjectConverter<ChargeType>)
        public ChargeType ChargeType { get; set; }

        [DynamoDBProperty(AttributeName = "debit_code")]
        public string DebitCode { get; set; }

        [DynamoDBProperty(AttributeName = "debit_code_description")]
        public string DebitCodeDescription { get; set; }

        [DynamoDBProperty(AttributeName = "effective_start_date",Converter =typeof(DynamoDbDateTimeConverter))]
        public DateTime EffectiveStartDate { get; set; } 

        [DynamoDBProperty(AttributeName = "termination_date", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime TerminationDate { get; set; }

        [DynamoDBProperty(AttributeName = "period_code")]
        public string PeriodCode { get; set; }

        [DynamoDBProperty(AttributeName = "debit_next_due", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime DebitNextDue { get; set; }

        [DynamoDBProperty(AttributeName = "debit_last_charged", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime DebitLastCharged { get; set; }

        [DynamoDBProperty(AttributeName = "debit_active")]
        public bool DebitActive { get; set; }

        [DynamoDBProperty(AttributeName = "debit_value")]
        public decimal DebitValue { get; set; }

        [DynamoDBProperty(AttributeName = "property_debit")]
        public bool PropertyDebit { get; set; }

        [DynamoDBProperty(AttributeName = "debit_source")]
        public string DebitSource { get; set; }

        [DynamoDBProperty(AttributeName = "timestamp", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime TimeStamp { get; set; }

        [DynamoDBProperty(AttributeName = "service_charge_schedule")]
        public string ServiceChargeSchedule { get; set; }

        [DynamoDBProperty(AttributeName = "data_import_source")]
        public string DataImportSource { get; set; }

        [DynamoDBProperty(AttributeName = "charge_details", Converter = (typeof(DynamoDbObjectListConverter<ChargeDetails>)))]
        public IEnumerable<ChargeDetails> ChargeDetails { get; set; }
    }
}
