using System.Collections.Generic;
using System.Linq;
using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Domain;

namespace ChargeApi.V1.Factories
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
                TargetType = domain.TargetType
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
        public static List<ChargeResponse> ToResponse(this IEnumerable<Charge> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
