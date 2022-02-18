using Amazon.DynamoDBv2.Model;
using ChargesApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Factories
{
    public static class DetailedChargesFactory
    {
        public static DetailedCharges ToDetailedCharge(this Dictionary<string, AttributeValue> scanResponseItem) => new DetailedCharges
        {
            Type = scanResponseItem["type"].S,
            ChargeType = Enum.Parse<ChargeType>(scanResponseItem["charge_type"].S),
            ChargeCode = scanResponseItem["charge_code"].S,
            Frequency = scanResponseItem["frequency"].S,
            Amount = decimal.Parse(scanResponseItem["amount"].N),
            StartDate  = DateTime.Parse(scanResponseItem["start_date"].S),
            EndDate = DateTime.Parse(scanResponseItem["end_date"].S)
        };
    }
}
