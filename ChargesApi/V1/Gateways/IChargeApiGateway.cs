using ChargeApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargeApi.V1.Gateways
{
    public interface IChargeApiGateway
    {
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