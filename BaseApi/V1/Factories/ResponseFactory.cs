using System.Collections.Generic;
using System.Linq;
using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Domain;

namespace ChargeApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static ChargeResponseObject ToResponse(this Charge domain)
        {
            return new ChargeResponseObject()
            {
                Id = domain.Id,
                TargetId = domain.TargetId,
                ChargeDetails = domain.ChargeDetails,
                TargetType = domain.TargetType
            };
        }

        public static List<ChargeResponseObject> ToResponse(this IEnumerable<Charge> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        } 
    }
}
