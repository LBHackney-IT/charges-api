using ChargesApi.V1.Domain;
using ChargesApi.V1.Infrastructure.Entities;
using System;
using System.Collections.Generic;

namespace ChargesApi.V1.Factories
{
    public static class ActualsFactory
    {
        public static List<ActualsDbEntity> ToDatabase(this List<Actual> actuals)
        {
            if (actuals == null)
            {
                return null;
            }
            var response = new List<ActualsDbEntity>();
            actuals.ForEach(item =>
            {
                var estimateItem = new ActualsDbEntity
                {
                    Id = Guid.NewGuid(),
                    LeaseholderName = item.Name,
                    Prn = item.Prn,
                    BlockName = item.BlockName,
                    EstateName = item.EstateName,
                    MonthlyAmount = item.MonthlyAmount,
                    YearlyAmount = item.YearlyAmount,
                    ActualYear = item.ActualYear,
                    CreatedAt = DateTime.UtcNow
                };
                response.Add(estimateItem);
            });
            return response;
        }
    }
}
