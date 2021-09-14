using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;

namespace ChargesApi.V1.Factories
{
    public interface ISnsFactory
    {
        ChargesSns Create(AddChargesUpdateRequest chargeUpdate);
    }
}
