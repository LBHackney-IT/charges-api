using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class AddEstimatesUseCase : IAddEstimatesUseCase
    {
        private readonly IEstimatesApiGateway _estimatesApiGateway;

        public AddEstimatesUseCase(IEstimatesApiGateway estimatesApiGateway)
        {
            _estimatesApiGateway = estimatesApiGateway;
        }

        public async Task<int> AddEstimates(IFormFile file)
        {
            List<Estimate> estimates = new List<Estimate>();
            int processingCount = 0;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 1;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    
                    while (reader.Read()) //Each row of the file
                    {
                        if (processingCount != 0)
                        {
                            estimates.Add(new Estimate
                            {
                                Name = reader.GetValue(0).ToString(),
                                Prn = reader.GetValue(1).ToString(),
                                BlockName = reader.GetValue(2).ToString(),
                                EstateName = reader.GetValue(3).ToString(),
                                MonthlyAmount = Convert.ToDecimal(reader.GetValue(4)),
                                YearlyAmount = Convert.ToDecimal(reader.GetValue(5)),
                                EstimateYear = Convert.ToInt16(reader.GetValue(6))
                            });
                        }
                        processingCount++;
                    }
                }
            }

            var result = await _estimatesApiGateway.SaveEstimateBatch(estimates).ConfigureAwait(false);
            if (result)
                return processingCount - 1;
            else
                return 0;
        }
    }
}
