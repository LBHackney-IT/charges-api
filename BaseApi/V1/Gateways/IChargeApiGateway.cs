using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ChargeApi.V1.Domain;

namespace ChargeApi.V1.Gateways
{
    public interface IChargeApiGateway
    {
        public Task<Charge> GetChargeByIdAsync(Guid id);

        public Task<List<Charge>> GetAllChargesAsync(string type, Guid targetid);

        public Task AddAsync(Charge charge);

        public Task RemoveAsync(Charge charge);

        public Task UpdateAsync(Charge charge);
    }
}
