using ChargesApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways
{
    public interface IChargesListApiGateway
    {
        public Task<ChargesList> GetChargesListByIdAsync(Guid id);

        public Task<List<ChargesList>> GetAllChargesListAsync(string chargeGroup, string chargeType);

        public Task AddAsync(ChargesList chargesList);
    }
}
