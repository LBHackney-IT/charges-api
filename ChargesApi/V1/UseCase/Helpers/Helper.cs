using ChargesApi.V1.Domain;
using Hackney.Shared.Tenure.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Helpers
{
    public static class Helper
    {
        public static DateTime GetFirstMondayForApril(int year)
        {
            var dt = new DateTime(year, 4, 1);
            while (dt.DayOfWeek != DayOfWeek.Monday)
            {
                dt = dt.AddDays(1);
            }
            return dt;
        }

        public static int GetLeaseholdersCount(List<EstimateCharge> estimateCharges)
        {
            if (estimateCharges == null || !estimateCharges.Any()) return 0;
            var filteredData = estimateCharges.Where(
                   x => x.TenureType == TenureTypes.LeaseholdRTB.Description
                || x.TenureType == TenureTypes.PrivateSaleLH.Description
                || x.TenureType == TenureTypes.SharedOwners.Description
                || x.TenureType == TenureTypes.SharedEquity.Description
                || x.TenureType == TenureTypes.ShortLifeLse.Description
                || x.TenureType == TenureTypes.LeaseholdStair.Description
            );
            return filteredData.Count();

        }
        public static int GetFreeholdersCount(List<EstimateCharge> estimateCharges)
        {
            if (estimateCharges == null || !estimateCharges.Any()) return 0;
            var filteredData = estimateCharges.Where(
                   x => x.TenureType == TenureTypes.FreeholdServ.Description
            );
            return filteredData.Count();

        }
        public static async Task<byte[]> GetBytes(this IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream).ConfigureAwait(false);
                return memoryStream.ToArray();
            }
        }
    }
}
