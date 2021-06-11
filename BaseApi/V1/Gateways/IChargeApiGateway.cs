using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ChargeApi.V1.Domain;

namespace ChargeApi.V1.Gateways
{
    public interface IChargeApiGateway
    {
        public Charge GetChargeById(Guid id);
        public Task<Charge> GetChargeByIdAsync(Guid id);

        public Task<List<Charge>> GetAllChargesAsync(string type, Guid targetid);

        public void Add(Charge charge);
        public Task AddAsync(Charge charge);
        public void AddRange(List<Charge> charges);
        public Task AddRangeAsync(List<Charge> charges);

        public void Remove(Charge charge);
        public Task RemoveAsync(Charge charge);
        public void RemoveRange(List<Charge> charges);
        public Task RemoveRangeAsync(List<Charge> charges);

        public void Update(Charge charge);
        public Task UpdateAsync(Charge charge);
    }
}
