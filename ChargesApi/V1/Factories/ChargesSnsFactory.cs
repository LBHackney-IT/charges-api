using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using System;
using ChargesApi.V1.Infrastructure.AppConstants;
using System.Collections.Generic;

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

        public ChargesUpdateSns Update(IEnumerable<DetailedChargesUpdateDomain> chargeMessage, Guid chargeId, Guid targetId) => new ChargesUpdateSns
        {
            CorrelationId = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            EntityId = chargeId,
            EntityTargetId = targetId,
            Id = Guid.NewGuid(),
            EventType = ChargeUpdateEventConstants.DWELLINGCHARGEUPDATEDTYPE,
            Version = ChargeUpdateEventConstants.V1VERSION,
            SourceDomain = EventCreationConstants.SOURCEDOMAIN,
            SourceSystem = EventCreationConstants.SOURCESYSTEM,
            EventData = new EventData
            {
                NewData = chargeMessage
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

        public ChargesSns UploadPrintRentRoomMessage(PropertyChargesQueryParameters queryParameters)
        {
            return new ChargesSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                EventType = PrintRentRoomUploadConstants.EVENTTYPE,
                Version = PrintRentRoomUploadConstants.V1VERSION,
                SourceDomain = EventCreationConstants.SOURCEDOMAIN,
                SourceSystem = EventCreationConstants.SOURCESYSTEM,
                EventData = new EventData
                {
                    NewData = queryParameters
                },
                User = new User { Name = EventCreationConstants.NAME, Email = EventCreationConstants.EMAIL }
            };
        }
    }
}
