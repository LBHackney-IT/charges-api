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
            
            if (charges.Any())
            {
                var result = new ChargesSummaryResponse();
                var tenantChargesList = charges.Where(x => x.ChargeGroup == ChargeGroup.tenants).ToList();
                if (tenantChargesList.Any())
                {
                    result.TargetId = tenantChargesList.First().TargetId;
                    result.TargetType = tenantChargesList.First().TargetType;
                    var tenantsCharges = new List<ChargeDetail>();
                    tenantChargesList.ForEach(chargeItem =>
                    {
                        tenantsCharges.AddRange(GetChargesList(chargeItem.DetailedCharges));
                    });
                    result.TenantsCharges = tenantsCharges;
                }
                var leaseholdersChargesList = charges.Where(x => x.ChargeGroup == ChargeGroup.leaseholders).ToList();
                if (leaseholdersChargesList.Any())
                {
                    var leaseholdersCharges = new List<ChargeDetail>();
                    leaseholdersChargesList.ForEach(chargeItem =>
                    {
                        leaseholdersCharges.AddRange(GetChargesList(chargeItem.DetailedCharges));
                    });
                    result.LeaseholdersCharges = leaseholdersCharges;
                }
                return result;
            }
            else return null;
        }
        private static List<ChargeDetail> GetChargesList(IEnumerable<DetailedCharges> chargeDetails)
        {
            var chargesList = new List<ChargeDetail>();
            if (chargeDetails.Any())
            {
                chargesList = chargeDetails.Where(x => x.Type == Constants.ServiceChargeType).Select(charge => charge.ToResponse()).ToList();
            }
            return chargesList;
        }
    }
}
