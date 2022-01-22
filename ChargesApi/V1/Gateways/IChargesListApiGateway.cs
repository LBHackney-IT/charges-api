using ChargesApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways
{
    public interface IChargesListApiGateway
    {
        public Task<ChargesList> GetChargesListByIdAsync(Guid id, string chargeCode);

        public Task<List<ChargesList>> GetAllChargesListAsync(string chargeCode);

        public Task AddAsync(ChargesList chargesList, string token);
    }
}
