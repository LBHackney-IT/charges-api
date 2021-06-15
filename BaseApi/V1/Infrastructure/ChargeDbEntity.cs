using Amazon.DynamoDBv2.DataModel;
using ChargeApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChargeApi.V1.Infrastructure
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

        [DynamoDBProperty(AttributeName = "detailed_charges", Converter = (typeof(DynamoDbObjectListConverter<DetailedCharges>)))]
        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }
    }
}
