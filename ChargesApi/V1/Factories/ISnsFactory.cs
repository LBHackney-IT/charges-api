using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Factories
{
    public interface ISnsFactory
    {
        ChargesSns Create(ChargeResponse chargeResponse);
        ChargesSns Create(AddChargesUpdateRequest chargeUpdate);
        ChargesSns CreateFileUploadMessage(FileLocationResponse location);
        ChargesUpdateSns Update(IEnumerable<DetailedChargesUpdateDomain> chargeMessage, Guid chargeId, Guid targetId);
        ChargesSns UploadPrintRentRoomMessage(PropertyChargesQueryParameters queryParameters);
    }
}
