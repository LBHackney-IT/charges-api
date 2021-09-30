using ChargesApi.V1.Boundary.Request;
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
    public class UpdateUseCase : IUpdateUseCase
    {
        private readonly IChargesApiGateway _gateway;

        public UpdateUseCase(IChargesApiGateway gateway)
        {
            _gateway = gateway;
        }

        public ChargeResponse Execute(UpdateChargeRequest charge)
        {
            if (charge == null)
            {
                throw new ArgumentNullException(nameof(charge));
            }

            var domain = charge.ToDomain();

            _gateway.Update(domain);
            return domain.ToResponse();
        }

        public async Task<ChargeResponse> ExecuteAsync(UpdateChargeRequest charge)
        {
            if (charge == null)
            {
                throw new ArgumentNullException(nameof(charge));
            }
            var existingCharge = await _gateway.GetChargeByIdAsync(charge.Id).ConfigureAwait(false);
            existingCharge.DetailedCharges = GetModifiedChargeDetails(existingCharge, charge);

            await _gateway.UpdateAsync(existingCharge).ConfigureAwait(false);

            return existingCharge.ToResponse();
        }

        private static List<DetailedCharges> GetModifiedChargeDetails(Charge existingCharge, UpdateChargeRequest charge)
        {
            var detailedCharges = new List<DetailedCharges>();
            if (existingCharge.DetailedCharges != null && existingCharge.DetailedCharges.Any())
            {
                foreach (var item in existingCharge.DetailedCharges)
                {
                    if (item.EndDate.Year >= DateTime.UtcNow.Year)
                    {
                        detailedCharges.Add(item);
                    }
                }
                if (charge.DetailedCharges != null && charge.DetailedCharges.Any())
                {
                    foreach (var item in charge.DetailedCharges)
                    {
                        var chargeDetail = detailedCharges.Where(x => x.ChargeCode.ToLower() == item.ChargeCode.ToLower());
                        if (!chargeDetail.Any())
                        {
                            detailedCharges.Add(item);
                        }
                        else
                        {
                            var chargeItem = chargeDetail.Where(x => x.EndDate >= item.StartDate && x.StartDate <= item.StartDate );

                            if(!chargeItem.Any())
                                detailedCharges.Add(item);
                        }
                    }
                }
            }
            else
            {
                if (charge.DetailedCharges != null && charge.DetailedCharges.Any())
                {
                    foreach (var item in charge.DetailedCharges)
                    {
                        if (item.EndDate > DateTime.UtcNow)
                        {
                            detailedCharges.Add(item);
                        }
                    }
                }
            }
            return detailedCharges;
        }
    }
}
