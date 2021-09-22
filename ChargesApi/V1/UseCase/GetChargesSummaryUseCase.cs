using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class GetChargesSummaryUseCase : IGetChargesSummaryUseCase
    {
        private readonly IChargesApiGateway _chargesApiGateway;

        public GetChargesSummaryUseCase(IChargesApiGateway chargesApiGateway)
        {
            _chargesApiGateway = chargesApiGateway;
        }

        public async Task<ChargesSummaryResponse> ExecuteAsync(Guid targetId, string targetType)
        {
            var charges = await _chargesApiGateway.GetAllChargesAsync(targetType, targetId).ConfigureAwait(false);
            var result = new ChargesSummaryResponse();
            if (charges.Any())
            {
                result.TargetId = charges.First().TargetId;
                result.TargetType = charges.First().TargetType;
                var chargesList = new List<ChargeDetail>();
                charges.ForEach(chargeItem =>
                {
                    var chargesListResult = GetChargesList(chargeItem.DetailedCharges, chargeItem.ChargeGroup);
                    if (chargesListResult.Any())
                    {
                        chargesList.AddRange(chargesListResult);
                    }
                });
                result.ChargesList = chargesList;
                return result;
            }
            else return null;
        }
        private static List<ChargeDetail> GetChargesList(IEnumerable<DetailedCharges> chargeDetails, ChargeGroup chargeGroup)
        {
            var chargesList = new List<ChargeDetail>();
            if (chargeDetails.Any())
            {
                var serviceChargesList = chargeDetails.Where(x => x.Type == Constants.ServiceChargeType).ToList();

                if (serviceChargesList.Any())
                {
                    serviceChargesList.ForEach(chargeDetailItem =>
                    {
                        var chargeDetail = chargeDetailItem.ToResponse();
                        chargeDetail.ChargeGroup = chargeGroup;
                        chargesList.Add(chargeDetail);
                    });
                }
            }
            return chargesList;
        }
    }
}
