using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;

namespace ChargesApi.V1.Factories
{
    public interface ISnsFactory
    {
        ChargesSns Create(ChargeResponse chargeResponse);
        ChargesSns Create(AddChargesUpdateRequest chargeUpdate);
        ChargesSns CreateFileUploadMessage(FileLocationResponse location);
    }
}
