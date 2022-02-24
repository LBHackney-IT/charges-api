using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.Infrastructure.JWT;
using ChargesApi.V1.UseCase.Interfaces;

namespace ChargesApi.V1.UseCase
{
    public class UpdateChargeUseCase : IUpdateChargeUseCase
    {
        private readonly IChargesApiGateway _chargesApiGateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public UpdateChargeUseCase(IChargesApiGateway chargesApiGateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _chargesApiGateway = chargesApiGateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<ChargeResponse> ExecuteAsync(Guid targetId, ChargesUpdateDomain chargesUpdateDomain, string token)
        {
            // Retrieve collection of charges
            var charges = await _chargesApiGateway.GetAllChargesAsync(targetId).ConfigureAwait(false);

            if (charges == null)
                throw new ArgumentException($"No Charge by Id cannot be found!");

            // Get single charge by chargeSubGroup and chargeYear
            var singleCharge = charges.FirstOrDefault(x => x.ChargeYear == chargesUpdateDomain.ChargeYear
                                                           && x.ChargeSubGroup == chargesUpdateDomain.ChargeSubGroup);

            if (singleCharge == null)
                throw new ArgumentNullException(nameof(chargesUpdateDomain));

            var changeLogs = new Dictionary<string, decimal>();

            // Update the existing detailed charge
            foreach (var requestedDetailedCharge in chargesUpdateDomain.DetailedCharges)
            {
                var existingDetailedCharge = singleCharge.DetailedCharges.FirstOrDefault(x =>
                    x.SubType == requestedDetailedCharge.SubType && x.ChargeType == requestedDetailedCharge.ChargeType);

                if (existingDetailedCharge != null)
                {
                    changeLogs.Add(existingDetailedCharge.SubType, (requestedDetailedCharge.Amount - existingDetailedCharge.Amount));
                    existingDetailedCharge.Amount = requestedDetailedCharge.Amount;
                }
            }

            singleCharge.CreatedBy = Helper.GetUserName(token);
            singleCharge.LastUpdatedAt = DateTime.UtcNow;
            await _chargesApiGateway.UpdateAsync(singleCharge).ConfigureAwait(false);

            var updateList = GetDetailChargeChangeList(changeLogs, singleCharge);
            var snsMessage = _snsFactory.Update(updateList, singleCharge.Id, singleCharge.TargetId);
            await _snsGateway.PublishUpdate(snsMessage).ConfigureAwait(false);

            return singleCharge?.ToResponse();
        }
        private static IEnumerable<DetailedChargesUpdateDomain> GetDetailChargeChangeList(Dictionary<string, decimal> changeLogs, Charge charge)
        {
            var detailChargesToUpdate = new List<DetailedChargesUpdateDomain>();

            charge.DetailedCharges.ToList().ForEach(x =>
           {
               if (changeLogs.ContainsKey(x.SubType))
               {
                   var data = new DetailedChargesUpdateDomain
                   {
                       ChargeType = x.ChargeType,
                       SubType = x.SubType,
                       DifferenceAmount = changeLogs[x.SubType]
                   };
                   detailChargesToUpdate.Add(data);
               }
           });
            return detailChargesToUpdate.AsEnumerable();
        }
    }
}
