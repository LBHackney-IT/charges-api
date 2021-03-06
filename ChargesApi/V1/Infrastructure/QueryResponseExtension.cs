using Amazon.DynamoDBv2.Model;
using ChargesApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;

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
                if (item.ContainsKey("detailed_charges") && !item["detailed_charges"].NULL)
                {
                    var innerItem = item["detailed_charges"].L;
                    foreach (var detail in innerItem)
                    {
                        detailCharges.Add(new DetailedCharges
                        {
                            Amount = Convert.ToDecimal(detail.M["amount"].N, CultureInfo.InvariantCulture),
                            ChargeCode = detail.M["chargeCode"].S,
                            ChargeType = Enum.Parse<ChargeType>(detail.M["chargeType"].S),
                            Type = detail.M["type"].S,
                            SubType = detail.M["subType"].S,
                            Frequency = detail.M["frequency"].S,
                            StartDate = DateTime.Parse(detail.M["startDate"].S),
                            EndDate = DateTime.Parse(detail.M["endDate"].S)
                        });
                    }
                }

                Charge charge = new Charge
                {
                    Id = Guid.Parse(item["id"].S),
                    TargetId = Guid.Parse(item["target_id"].S),
                    ChargeGroup = Enum.Parse<ChargeGroup>(item["charge_group"].S),
                    ChargeSubGroup =
                        item.ContainsKey("charge_sub_group")
                            ? Enum.Parse<ChargeSubGroup>(item["charge_sub_group"].S)
                            : ((ChargeSubGroup?) null),
                    TargetType = Enum.Parse<TargetType>(item["target_type"].S),
                    ChargeYear = (item.ContainsKey("charge_year") && !item["charge_year"].NULL)
                    ? Convert.ToInt16(item["charge_year"].N) : (short) 0
                };

                if (detailCharges.Count > 0)
                    charge.DetailedCharges = detailCharges;

                chargesList.Add(charge);
            }

            return chargesList;
        }
        public static ChargeKeys GetChargeKeys(this Charge charge)
            => new ChargeKeys(charge.Id, charge.TargetId);

        public static List<Charge> ToChargeDomain(this ScanResponse response)
        {
            var chargesList = new List<Charge>();
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                var detailCharges = new List<DetailedCharges>();
                var innerItem = item.ContainsKey("detailed_charges") ? item["detailed_charges"].L : null;

                if (innerItem != null)
                {
                    foreach (var detail in innerItem)
                    {
                        detailCharges.Add(new DetailedCharges
                        {
                            Amount = Convert.ToDecimal(detail.M["amount"].N, CultureInfo.InvariantCulture),
                            ChargeCode = detail.M["chargeCode"].S,
                            ChargeType = Enum.Parse<ChargeType>(detail.M["chargeType"].S),
                            Type = detail.M["type"].S,
                            SubType = detail.M["subType"].S,
                            Frequency = detail.M["frequency"].S,
                            StartDate = DateTime.Parse(detail.M["startDate"].S),
                            EndDate = DateTime.Parse(detail.M["endDate"].S)
                        });
                    }
                }

                chargesList.Add(new Charge
                {
                    Id = Guid.Parse(item["id"].S),
                    TargetId = Guid.Parse(item["target_id"].S),
                    ChargeGroup = Enum.Parse<ChargeGroup>(item["charge_group"].S),
                    ChargeSubGroup = item.ContainsKey("charge_sub_group") ? Enum.Parse<ChargeSubGroup>(item["charge_sub_group"].S) : ((ChargeSubGroup?) null),
                    TargetType = Enum.Parse<TargetType>(item["target_type"].S),
                    ChargeYear = item.ContainsKey("charge_year") ? Convert.ToInt16(item["charge_year"].N) : Convert.ToInt16(0),
                    DetailedCharges = detailCharges
                });
            }

            return chargesList;
        }
    }
}
