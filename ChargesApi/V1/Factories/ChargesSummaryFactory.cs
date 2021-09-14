using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;

namespace ChargesApi.V1.Factories
{
    public static class ChargesSummaryFactory
    {
        public static ChargeDetail ToResponse(this DetailedCharges domain)
        {
            return new ChargeDetail
            {
                ChargeAmount = domain.Amount,
                ChargeCode = domain.ChargeCode,
                ChargeName = domain.SubType,
                ChargeYear = domain.StartDate.Year
            };
        }
    }
}
