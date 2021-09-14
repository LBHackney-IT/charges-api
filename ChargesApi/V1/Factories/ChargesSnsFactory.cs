using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.JWT;
using System;

namespace ChargesApi.V1.Factories
{
    public class ChargesSnsFactory : ISnsFactory
    {
        public ChargesSns Create(AddChargesUpdateRequest chargeUpdate)
        {
            return new ChargesSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                EventType = CreateEventConstants.EVENTTYPE,
                Version = CreateEventConstants.V1VERSION,
                SourceDomain = CreateEventConstants.SOURCEDOMAIN,
                SourceSystem = CreateEventConstants.SOURCESYSTEM,
                EventData = new EventData
                {
                    NewData = chargeUpdate
                },
                User = new User { Name = CreateEventConstants.NAME, Email = CreateEventConstants.EMAIL }
            };
        }
    }
}
