using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using System;
using ChargesApi.V1.Infrastructure.AppConstants;

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
                EventType = ChargeUpdateEventConstants.EVENTTYPE,
                Version = ChargeUpdateEventConstants.V1VERSION,
                SourceDomain = EventCreationConstants.SOURCEDOMAIN,
                SourceSystem = EventCreationConstants.SOURCESYSTEM,
                EventData = new EventData
                {
                    NewData = chargeUpdate
                },
                User = new User { Name = EventCreationConstants.NAME, Email = EventCreationConstants.EMAIL }
            };
        }

        public ChargesSns Create(ChargeResponse chargeResponse) => new ChargesSns
        {
            CorrelationId = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            EntityId = chargeResponse.Id,
            Id = Guid.NewGuid(),
            EventType = ChargeUpdateEventConstants.DWELLINGCHARGEUPDATEDTYPE,
            Version = ChargeUpdateEventConstants.V1VERSION,
            SourceDomain = EventCreationConstants.SOURCEDOMAIN,
            SourceSystem = EventCreationConstants.SOURCESYSTEM,
            EventData = new EventData
            {
                NewData = chargeResponse
            },
            User = new User { Name = EventCreationConstants.NAME, Email = EventCreationConstants.EMAIL }
        };

        public ChargesSns CreateFileUploadMessage(FileLocationResponse location)
        {
            return new ChargesSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                EventType = FileUploadEventConstants.EVENTTYPE,
                Version = FileUploadEventConstants.V1VERSION,
                SourceDomain = EventCreationConstants.SOURCEDOMAIN,
                SourceSystem = EventCreationConstants.SOURCESYSTEM,
                EventData = new EventData
                {
                    NewData = location
                },
                User = new User { Name = EventCreationConstants.NAME, Email = EventCreationConstants.EMAIL }
            };
        }
    }
}
