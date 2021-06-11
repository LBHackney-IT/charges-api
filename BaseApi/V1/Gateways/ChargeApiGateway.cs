using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChargeApi.V1.Domain;
using ChargeApi.V1.Factories;
using ChargeApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ChargeApi.V1.Gateways
{
    //TODO: Rename to match the data source that is being accessed in the gateway eg. MosaicGateway
    public class ChargeApiGateway : IChargeApiGateway
    {
        private readonly ChargeContext _chargeDbContext;

        public ChargeApiGateway(ChargeContext chargeDbContext)
        {
            _chargeDbContext = chargeDbContext;
        }

        public Charge GetChargeById(Guid id)
        {
            var result = _chargeDbContext.ChargeEntities.Find(id);

            return result?.ToDomain();
        }

        public List<Charge> GetAllCharges(string type, Guid targetid)
        {
            TargetType targetType;
            if (Enum.TryParse(type, out targetType))
            {
                IQueryable<ChargeDbEntity> data = _chargeDbContext
                .ChargeEntities
                .Where(p => p.TargetType == targetType && p.TargetId == targetid);
                return data.Select(s => s.ToDomain()).ToList();
            }
            else
                throw new ArgumentException("Invalid type");
        }

        public async Task<List<Charge>> GetAllChargesAsync(string type, Guid targetid)
        {
            TargetType targetType;
            if (Enum.TryParse(type, out targetType))
            {
                IQueryable<ChargeDbEntity> data = _chargeDbContext
                  .ChargeEntities
                  .Where(p => p.TargetType == targetType && p.TargetId == targetid);
                return await data.Select(s => s.ToDomain()).ToListAsync().ConfigureAwait(false);
            }
            else
                throw new ArgumentException("Invalid type");
        }

        public void Add(Charge charge)
        {
            _chargeDbContext.ChargeEntities.Add(charge.ToDatabase());
        }

        public void Remove(Charge charge)
        {
            _chargeDbContext.ChargeEntities.Remove(charge.ToDatabase());
        }

        public void Update(Charge charge)
        {
            _chargeDbContext.Entry(charge).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _chargeDbContext.SaveChanges();
        }

        public async Task UpdateAsync(Charge charge)
        {
            _chargeDbContext.Entry(charge).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _chargeDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Charge> GetChargeByIdAsync(Guid id)
        {
            var result = await _chargeDbContext.ChargeEntities.FindAsync(id).ConfigureAwait(false);
            return result?.ToDomain();
        }

        public async Task AddAsync(Charge charge)
        {
            await _chargeDbContext.AddAsync(charge).ConfigureAwait(false);
        }

        public void RemoveRange(List<Charge> charges)
        {
            _chargeDbContext.RemoveRange(charges);
        }

        public void AddRange(List<Charge> charges)
        {
            _chargeDbContext.AddRange(charges);
        }

        public async Task AddRangeAsync(List<Charge> charges)
        {
            await _chargeDbContext.AddRangeAsync(charges).ConfigureAwait(false);
        }

        public async Task RemoveAsync(Charge charge)
        {
            _chargeDbContext.Remove(charge);
            await _chargeDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveRangeAsync(List<Charge> charges)
        {
            foreach (Charge c in charges)
            {
                await RemoveAsync(c).ConfigureAwait(false);
            }
        }

        public void CalculateCharges(Guid targetId, string targetType)
        {
            throw new NotImplementedException();
        }

        public Task CalculateChargesAsync(Guid targetId, string targetType)
        {
            throw new NotImplementedException();
        }
    }
}
