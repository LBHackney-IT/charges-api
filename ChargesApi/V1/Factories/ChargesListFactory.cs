using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.Entities;

namespace ChargesApi.V1.Factories
{
    public static class ChargesListFactory
    {
        public static ChargesList ToDomain(this ChargesListDbEntity chargesListEntity)
        {
            if (chargesListEntity == null)
            {
                return null;
            }

            return new ChargesList
            {
                Id = chargesListEntity.Id,
                ChargeCode = chargesListEntity.ChargeCode,
                ChargeGroup = chargesListEntity.ChargeGroup,
                ChargeName = chargesListEntity.ChargeName,
                ChargeType = chargesListEntity.ChargeType
            };
        }

        public static ChargesListDbEntity ToDatabase(this ChargesList chargesList)
        {
            if (chargesList == null)
            {
                return null;
            }

            return new ChargesListDbEntity
            {
                Id = chargesList.Id,
                ChargeCode = chargesList.ChargeCode,
                ChargeGroup = chargesList.ChargeGroup,
                ChargeName = chargesList.ChargeName,
                ChargeType = chargesList.ChargeType
            };
        }

        public static ChargesList ToDomain(this AddChargesListRequest chargesListRequest)
        {
            if (chargesListRequest == null)
            {
                return null;
            }

            return new ChargesList
            {
                ChargeCode = chargesListRequest.ChargeCode,
                ChargeGroup = chargesListRequest.ChargeGroup,
                ChargeName = chargesListRequest.ChargeName,
                ChargeType = chargesListRequest.ChargeType
            };
        }
    }
}
