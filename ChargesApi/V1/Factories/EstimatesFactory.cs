using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.Entities;
using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Factories
{
    public static class EstimatesFactory
    {
        public static List<EstimatesDbEntity> ToDatabase(this List<Estimate> estimates)
        {
            if (estimates == null)
            {
                return null;
            }
            var response = new List<EstimatesDbEntity>();
            estimates.ForEach(item =>
            {
                var estimateItem = new EstimatesDbEntity
                {
                    Id = Guid.NewGuid(),
                    LeaseholderName = item.Name,
                    Prn = item.Prn,
                    BlockName = item.BlockName,
                    EstateName = item.EstateName,
                    MonthlyAmount = item.MonthlyAmount,
                    YearlyAmount = item.YearlyAmount,
                    EstimateYear = item.EstimateYear,
                    CreatedAt = DateTime.UtcNow
                };
                response.Add(estimateItem);
            });
            return response;
        }
    }
}
