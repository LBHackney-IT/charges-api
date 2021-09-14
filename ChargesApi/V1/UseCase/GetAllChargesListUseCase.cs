using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class GetAllChargesListUseCase : IGetAllChargesListUseCase
    {
        private readonly IChargesListApiGateway _gateway;

        public GetAllChargesListUseCase(IChargesListApiGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<List<ChargesListResponse>> ExecuteAsync(string chargeGroup, string chargeType)
        {
            var result = new List<ChargesListResponse>();
            if (Enum.TryParse(chargeType, out ChargeType chargeTypeVal))
            {
                result = chargeTypeVal switch
                {
                    ChargeType.estate => (await _gateway.GetAllChargesListAsync(chargeGroup.ToLower(), chargeType.ToLower()).ConfigureAwait(false)).ToResponse(),
                    ChargeType.block => (await GetBlockCharges(chargeGroup.ToLower()).ConfigureAwait(false)),
                    ChargeType.property => (await GetPropertyCharges(chargeGroup.ToLower()).ConfigureAwait(false)),
                    _ => null
                };
            }
            return result;
        }

        private async Task<List<ChargesListResponse>> GetBlockCharges(string chargeGroup)
        {
            var blockChargesResult = new List<ChargesListResponse>();
            var estateCharges = (await _gateway.GetAllChargesListAsync(chargeGroup.ToLower(), ChargeType.estate.ToString()).ConfigureAwait(false)).ToResponse();
            blockChargesResult.AddRange(estateCharges);

            var blockCharges = (await _gateway.GetAllChargesListAsync(chargeGroup.ToLower(), ChargeType.block.ToString()).ConfigureAwait(false)).ToResponse();
            blockChargesResult.AddRange(blockCharges);
            return blockChargesResult;
        }
        private async Task<List<ChargesListResponse>> GetPropertyCharges(string chargeGroup)
        {
            var result = new List<ChargesListResponse>();
            result.AddRange(await GetBlockCharges(chargeGroup).ConfigureAwait(false));

            var propertyCharges = (await _gateway.GetAllChargesListAsync(chargeGroup.ToLower(), ChargeType.property.ToString()).ConfigureAwait(false)).ToResponse();
            result.AddRange(propertyCharges);

            return result;
        }
    }
}
