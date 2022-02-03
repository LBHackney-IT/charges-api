using System.Collections.Generic;
using System.Linq;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;

namespace ChargesApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static ChargeResponse ToResponse(this Charge domain)
        {
            return new ChargeResponse()
            {
                Id = domain.Id,
                TargetId = domain.TargetId,
                DetailedCharges = domain.DetailedCharges,
                TargetType = domain.TargetType,
                ChargeGroup = domain.ChargeGroup,
                ChargeYear = domain.ChargeYear
            };
        }

        public static UpdateChargeRequest ToUpdateModel(this ChargeResponse response)
        {
            return new UpdateChargeRequest
            {
                TargetId = response.TargetId,
                TargetType = response.TargetType,
                ChargeGroup = response.ChargeGroup,
                ChargeYear = response.ChargeYear,
                Id = response.Id,
                DetailedCharges = response.DetailedCharges
            };
        }
        public static ChargeResponse ToResponse(this UpdateChargeRequest updateChargeRequest)
        {
            return new ChargeResponse
            {
                TargetId = updateChargeRequest.TargetId,
                TargetType = updateChargeRequest.TargetType,
                ChargeGroup = updateChargeRequest.ChargeGroup,
                ChargeYear = updateChargeRequest.ChargeYear,
                Id = updateChargeRequest.Id,
                DetailedCharges = updateChargeRequest.DetailedCharges
            };
        }
        public static ChargeMaintenanceResponse ToResponse(this ChargeMaintenance domain)
        {
            return new ChargeMaintenanceResponse()
            {
                Id = domain.Id,
                ChargesId = domain.ChargesId,
                Reason = domain.Reason,
                Status = domain.Status,
                StartDate = domain.StartDate,
                ExistingValue = domain.ExistingValue,
                NewValue = domain.NewValue
            };
        }

        public static ChargesListResponse ToResponse(this ChargesList domain)
        {
            return new ChargesListResponse()
            {
                Id = domain.Id,
                ChargeType = domain.ChargeType,
                ChargeName = domain.ChargeName,
                ChargeGroup = domain.ChargeGroup,
                ChargeCode = domain.ChargeCode
            };
        }

        public static List<ChargeResponse> ToResponse(this IEnumerable<Charge> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
        public static List<ChargesListResponse> ToResponse(this IEnumerable<ChargesList> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
