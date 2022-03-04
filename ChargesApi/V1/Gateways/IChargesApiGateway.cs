using ChargesApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;

namespace ChargesApi.V1.Gateways
{
    public interface IChargesApiGateway
    {
        public Task<Charge> GetChargeByIdAsync(Guid targetId, Guid id);

        public Task<List<Charge>> GetAllChargesAsync(Guid targetId);

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

        public Task<bool> AddBatchAsync(List<Charge> charges);

        public Task<bool> AddTransactionBatchAsync(List<Charge> charges);

        public Task DeleteBatchAsync(IEnumerable<ChargeKeys> chargeIds, int batchCapacity);

        Task<IList<Charge>> GetChargesAsync(PropertyChargesQueryParameters queryParameters);
    }
}
