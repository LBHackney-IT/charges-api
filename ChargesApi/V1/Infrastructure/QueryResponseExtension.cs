using Amazon.DynamoDBv2.Model;
using ChargesApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Infrastructure
{
    public static class QueryResponseExtension
    {
        public static List<ChargesList> ToChargesListDomain(this QueryResponse response)
        {
            var chargesList = new List<ChargesList>();
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                chargesList.Add(new ChargesList
                {
                    Id = Guid.Parse(item["id"].S),
                    ChargeCode = item["charge_code"].S,
                    ChargeGroup = Enum.Parse<ChargeGroup>(item["charge_group"].S),
                    ChargeName = item["charge_name"].S,
                    ChargeType = Enum.Parse<ChargeType>(item["charge_type"].S),
                });
            }

            return chargesList;
        }
        public static List<Charge> ToChargeDomain(this QueryResponse response)
        {
            var chargesList = new List<Charge>();
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                var detailCharges = new List<DetailedCharges>();
                var innerItem = item["detailed_charges"].L;
                foreach (var detail in innerItem)
                {
                    var chargeType = (ChargeType) int.Parse(detail.M["chargeType"].N);
                    detailCharges.Add(new DetailedCharges
                    {
                        Amount = Convert.ToDecimal(detail.M["amount"].N),
                        ChargeCode = detail.M["chargeCode"].S,
                        ChargeType = chargeType,
                        Type = detail.M["type"].S,
                        SubType = detail.M["subType"].S,
                        Frequency = detail.M["frequency"].S,
                        StartDate = DateTime.Parse(detail.M["startDate"].S),
                        EndDate = DateTime.Parse(detail.M["endDate"].S)
                    });
                }

                chargesList.Add(new Charge
                {
                    Id = Guid.Parse(item["id"].S),
                    TargetId = Guid.Parse(item["target_id"].S),
                    ChargeGroup = Enum.Parse<ChargeGroup>(item["charge_group"].S),
                    TargetType = Enum.Parse<TargetType>(item["target_type"].S),
                    ChargeYear = Convert.ToInt16(item["charge_year"].N),
                    DetailedCharges = detailCharges
                });
            }

            return chargesList;
        }
    }
}
