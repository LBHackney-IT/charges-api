using System.Collections.Generic;
using System.Linq;
using BaseApi.V1.Boundary.Response;
using BaseApi.V1.Domain;

namespace BaseApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static ChargeResponseObject ToResponse(this Charge domain)
        {
            return new ChargeResponseObject()
            {
                Id = domain.Id,
                TargetId = domain.TargetId,
                ChargeType = domain.ChargeType,
                ChargeDetails = domain.ChargeDetails,
                DataImportSource = domain.DataImportSource,
                DebitActive = domain.DebitActive,
                DebitCode = domain.DebitCode,
                DebitCodeDescription = domain.DebitCodeDescription,
                DebitLastCharged = domain.DebitLastCharged,
                DebitValue = domain.DebitValue,
                DebitNextDue = domain.DebitNextDue,
                DebitSource = domain.DebitSource,
                EffectiveStartDate = domain.EffectiveStartDate,
                PeriodCode = domain.PeriodCode,
                PropertyDebit = domain.PropertyDebit,
                ServiceChargeSchedule = domain.ServiceChargeSchedule,
                TerminationDate = domain.TerminationDate,
                TimeStamp = domain.TimeStamp
            };
        }

        public static List<ChargeResponseObject> ToResponse(this IEnumerable<Charge> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        } 
    }
}
