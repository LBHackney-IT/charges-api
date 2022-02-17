using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.Entities;

namespace ChargesApi.V1.Factories
{
    public static class ChargeFactory
    {
        public static Charge ToDomain(this ChargeDbEntity chargeEntity)
        {
            if (chargeEntity == null)
            {
                return null;
            }

            return new Charge
            {
                Id = chargeEntity.Id,
                TargetId = chargeEntity.TargetId,
                TargetType = chargeEntity.TargetType,
                ChargeYear = chargeEntity.ChargeYear,
                ChargeGroup = chargeEntity.ChargeGroup,
                ChargeSubGroup = chargeEntity.ChargeSubGroup,
                CreatedAt = chargeEntity.CreatedAt,
                CreatedBy = chargeEntity.CreatedBy,
                LastUpdatedAt = chargeEntity.LastUpdatedAt,
                LastUpdatedBy = chargeEntity.LastUpdatedBy,
                DetailedCharges = chargeEntity.DetailedCharges
            };
        }

        public static ChargeDbEntity ToDatabase(this Charge charge)
        {
            if (charge == null)
            {
                return null;
            }

            return new ChargeDbEntity
            {
                Id = charge.Id,
                TargetId = charge.TargetId,
                TargetType = charge.TargetType,
                ChargeGroup = charge.ChargeGroup,
                ChargeSubGroup = charge.ChargeSubGroup,
                ChargeYear = charge.ChargeYear,
                CreatedAt = charge.CreatedAt,
                CreatedBy = charge.CreatedBy,
                LastUpdatedBy = charge.LastUpdatedBy,
                LastUpdatedAt = charge.LastUpdatedAt,
                DetailedCharges = charge.DetailedCharges
            };
        }

        public static Charge ToDomain(this AddChargeRequest chargeRequest)
        {
            if (chargeRequest == null)
            {
                return null;
            }

            return new Charge
            {
                TargetId = chargeRequest.TargetId,
                TargetType = chargeRequest.TargetType,
                ChargeGroup = chargeRequest.ChargeGroup,
                ChargeSubGroup = chargeRequest.ChargeSubGroup,
                ChargeYear = chargeRequest.ChargeYear,
                DetailedCharges = chargeRequest.DetailedCharges
            };
        }
        public static Charge ToDomain(this ChargeResponse chargeResponse)
        {
            return new Charge
            {
                TargetId = chargeResponse.TargetId,
                TargetType = chargeResponse.TargetType,
                ChargeGroup = chargeResponse.ChargeGroup,
                ChargeSubGroup = chargeResponse.ChargeSubGroup,
                ChargeYear = chargeResponse.ChargeYear,
                Id = chargeResponse.Id,
                DetailedCharges = chargeResponse.DetailedCharges
            };
        }
        public static Charge ToDomain(this UpdateChargeRequest chargeRequest)
        {
            if (chargeRequest == null)
            {
                return null;
            }

            return new Charge
            {
                Id = chargeRequest.Id,
                TargetId = chargeRequest.TargetId,
                TargetType = chargeRequest.TargetType,
                ChargeGroup = chargeRequest.ChargeGroup,
                ChargeSubGroup = chargeRequest.ChargeSubGroup,
                ChargeYear = chargeRequest.ChargeYear,
                DetailedCharges = chargeRequest.DetailedCharges
            };
        }
        public static List<ChargeDbEntity> ToDatabaseList(this List<Charge> charges)
        {
            return charges.Select(item => item.ToDatabase()).ToList();
        }
        public static List<Charge> ToDomainList(this List<AddChargeRequest> charges)
        {
            return charges.Select(item => item.ToDomain()).ToList();
        }

        public static IEnumerable<WriteRequest> ToWriteRequests(this IEnumerable<Charge> charges) => charges.Select(c => new WriteRequest
        {
            DeleteRequest = new DeleteRequest
            {
                Key = new Dictionary<string, AttributeValue>
                {
                    { "id", new AttributeValue { S = c.Id.ToString() } }
                }
            }
        });

        public static Dictionary<string, AttributeValue> ToQueryRequest(this Charge charge)
        {
            return new Dictionary<string, AttributeValue>()
            {
                {"target_id", new AttributeValue {S = charge.TargetId.ToString()}},
                {"id", new AttributeValue {S = charge.Id.ToString()}},
                {"target_type", new AttributeValue {S = charge.TargetType.ToString()}},
                {"charge_group", new AttributeValue {S = charge.ChargeGroup.ToString()}},
                {"charge_sub_group", new AttributeValue {S = charge.ChargeSubGroup.ToString()}},
                {"charge_year", new AttributeValue {N = charge.ChargeYear.ToString()}},
                {
                    "detailed_charges", new AttributeValue
                            {
                                L = charge.DetailedCharges
                                    .Select(p =>
                                        new AttributeValue
                                        {
                                            M = new Dictionary<string, AttributeValue>
                                            {
                                                {"chargeCode", new AttributeValue {S = p.ChargeCode}},
                                                {"frequency", new AttributeValue {S = p.Frequency}},
                                                {"amount", new AttributeValue {N = p.Amount.ToString("F")}},
                                                {"endDate", new AttributeValue {S = p.EndDate.ToString(Constants.UtcDateFormat)}},
                                                {"chargeType", new AttributeValue {S = p.ChargeType.ToString()}},
                                                {"subType", new AttributeValue {S = p.SubType.ToString()}},
                                                {"type", new AttributeValue {S = p.Type.ToString()}},
                                                {"startDate", new AttributeValue {S = p.StartDate.ToString(Constants.UtcDateFormat)}}
                                            }
                                        }
                                    ).ToList()
                            }
                },
                {"created_by", new AttributeValue {S = charge.CreatedBy}},
                {"created_at", new AttributeValue {S = charge.CreatedAt.ToString("F")}}
            };
        }
    }
}
